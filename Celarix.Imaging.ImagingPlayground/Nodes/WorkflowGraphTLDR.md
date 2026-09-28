# Workflow Graph TL;DR

This layer separates the visual node editor from the executable workflow graph.

## Main Types

`STNode`

- Owns the visual/editor behavior: title, controls, ports, click handlers, etc.
- Implements `IWorkflowNodeCreator` so `WorkflowRunner` can build the real runtime node.
- Its `STNodeOption.DataType` is the actual payload type, such as `Image<Rgba32>`. There are no marker connection types.
- Port names still matter. Two ports can both be `Image<Rgba32>` and still mean different things, like `"Image"` vs. `"Mask"`.

`IWorkflowNode`

- Owns execution state and behavior.
- Receives connectors through `SetInputConnectors` and `SetOutputConnectors`.
- Reports graph edges through `GetInputs()` and `GetOutputs()`.
- Runs through `Run(CancellationToken)`.
- `Ready` should mean all required runtime prerequisites are satisfied.

`ValueConnector<T>`

- Carries one typed value from one producer to one or more consumers.
- `From` is the producing workflow node.
- `Tos` is fan-out: all downstream workflow nodes connected to this output.
- `Name` is copied from the ST output option text and is used with `Required<T>("Name")` / `Optional<T>("Name")`.
- `ResultAvailable` distinguishes "not produced yet" from a default/null-ish value.

## Graph Construction

`WorkflowRunner` does this:

1. Reads ST editor connections from output options.
2. Creates one workflow node per ST node using `IWorkflowNodeCreator`.
3. Builds typed `ValueConnector<T>` instances from `STNodeOption.DataType`.
4. Reuses the same output connector for fan-out from a single output port.
5. Adds that connector to every downstream node's input connector list.
6. Validates cycles, required inputs, and single-use node constraints.
7. Builds a topological sort and runs nodes in that order.

The important fan-out rule: one output port creates one `ValueConnector<T>`, and that connector has many `Tos`. Do not create a new connector per destination unless the semantics intentionally require separate computed values.

## Input/Output Properties

Workflow node connector properties should be marked explicitly:

- `[InputProperty]` means validation treats it as an input.
- `[OutputProperty]` means it is an output and should not be counted as a required input.
- `[OptionalInput]` on an input means it may be absent.

This avoids treating every `ValueConnector<T>` property as a required input.

## Adding A Node

1. Add an ST node class for editor UI and ports.
2. Give each `STNodeOption` the real payload type and a meaningful port name.
3. Implement `IWorkflowNodeCreator<TWorkflowNode>` on the ST node.
4. Add a workflow node class implementing `IWorkflowNode`.
5. In `SetInputConnectors`, use `connectors.Required<T>("PortName")` or `Optional<T>("PortName")`.
6. In `SetOutputConnectors`, capture output connectors by name.
7. In `Run`, read input connector results and call `SetValue` on output connectors.

## Things To Watch

- Port name mismatch is a runtime error, not a compile-time error.
- ST option type mismatch prevents connections or creates the wrong `ValueConnector<T>`.
- `GetOutputs()` drives cycle detection and topological sort, so it must reflect fan-out correctly.
- Singleton output surfaces, like the current viewer form, are fine for one active display target but will need revisiting for multiple simultaneous visual outputs.
