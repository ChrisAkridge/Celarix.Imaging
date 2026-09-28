using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ST.Library.UI.NodeEditor
{
    public class STNodeControlCollection: IList, ICollection, IEnumerable
    {
        /*
         * 为了确保安全在STNode中 仅继承者才能够访问集合
         */
        private int _Count;
        public int Count { get { return _Count; } }
        private STNodeControl[] m_controls;
        private STNode m_owner;

        internal STNodeControlCollection(STNode owner) {
            if (owner == null) throw new ArgumentNullException("所有者不能为空");
            m_owner = owner;
            m_controls = new STNodeControl[4];
        }

        public int Add(STNodeControl control) {
            if (control == null) throw new ArgumentNullException("添加对象不能为空");
            EnsureSpace(1);
            int nIndex = IndexOf(control);
            if (-1 == nIndex) {
                nIndex = _Count;
                control.Owner = m_owner;
                m_controls[_Count++] = control;
                Redraw();
            }
            return nIndex;
        }

        public void AddRange(STNodeControl[] controls) {
            if (controls == null) throw new ArgumentNullException("添加对象不能为空");
            EnsureSpace(controls.Length);
            foreach (var op in controls) {
                if (op == null) throw new ArgumentNullException("添加对象不能为空");
                if (-1 == IndexOf(op)) {
                    op.Owner = m_owner;
                    m_controls[_Count++] = op;
                }
            }
            Redraw();
        }

        public void Clear() {
            for (int i = 0; i < _Count; i++) m_controls[i].Owner = null;
            _Count = 0;
            m_controls = new STNodeControl[4];
            Redraw();
        }

        public bool Contains(STNodeControl option) {
            return IndexOf(option) != -1;
        }

        public int IndexOf(STNodeControl option) {
            return Array.IndexOf<STNodeControl>(m_controls, option);
        }

        public void Insert(int index, STNodeControl control) {
            if (index < 0 || index >= _Count)
                throw new IndexOutOfRangeException("索引越界");
            if (control == null)
                throw new ArgumentNullException("插入对象不能为空");
            EnsureSpace(1);
            for (int i = _Count; i > index; i--)
                m_controls[i] = m_controls[i - 1];
            control.Owner = m_owner;
            m_controls[index] = control;
            _Count++;
            Redraw();
        }

        public bool IsFixedSize {
            get { return false; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Remove(STNodeControl control) {
            int nIndex = IndexOf(control);
            if (nIndex != -1) RemoveAt(nIndex);
        }

        public void RemoveAt(int index) {
            if (index < 0 || index >= _Count)
                throw new IndexOutOfRangeException("索引越界");
            _Count--;
            m_controls[index].Owner = null;
            for (int i = index, Len = _Count; i < Len; i++)
                m_controls[i] = m_controls[i + 1];
            Redraw();
        }

        public STNodeControl this[int index] {
            get {
                if (index < 0 || index >= _Count)
                    throw new IndexOutOfRangeException("索引越界");
                return m_controls[index];
            }
            set { throw new InvalidOperationException("禁止重新赋值元素"); }
        }

        public void CopyTo(Array array, int index) {
            if (array == null)
                throw new ArgumentNullException("数组不能为空");
            m_controls.CopyTo(array, index);
        }

        public bool IsSynchronized {
            get { return true; }
        }

        public object SyncRoot {
            get { return this; }
        }

        public IEnumerator GetEnumerator() {
            for (int i = 0, Len = _Count; i < Len; i++)
                yield return m_controls[i];
        }
        /// <summary>
        /// 确认空间是否足够 空间不足扩大容量
        /// </summary>
        /// <param name="elements">需要增加的个数</param>
        private void EnsureSpace(int elements) {
            if (elements + _Count > m_controls.Length) {
                STNodeControl[] arrTemp = new STNodeControl[Math.Max(m_controls.Length * 2, elements + _Count)];
                m_controls.CopyTo(arrTemp, 0);
                m_controls = arrTemp;
            }
        }

        protected void Redraw() {
            if (m_owner != null && m_owner.Owner != null) {
                //m_owner.BuildSize();
                m_owner.Owner.Invalidate(m_owner.Owner.CanvasToControl(m_owner.Rectangle));
            }
        }
        //===================================================================================
        int IList.Add(object value) {
            return Add((STNodeControl)value);
        }

        void IList.Clear() {
            Clear();
        }

        bool IList.Contains(object value) {
            return Contains((STNodeControl)value);
        }

        int IList.IndexOf(object value) {
            return IndexOf((STNodeControl)value);
        }

        void IList.Insert(int index, object value) {
            Insert(index, (STNodeControl)value);
        }

        bool IList.IsFixedSize {
            get { return IsFixedSize; }
        }

        bool IList.IsReadOnly {
            get { return IsReadOnly; }
        }

        void IList.Remove(object value) {
            Remove((STNodeControl)value);
        }

        void IList.RemoveAt(int index) {
            RemoveAt(index);
        }

        object IList.this[int index] {
            get {
                return this[index];
            }
            set {
                this[index] = (STNodeControl)value;
            }
        }

        void ICollection.CopyTo(Array array, int index) {
            CopyTo(array, index);
        }

        int ICollection.Count {
            get { return _Count; }
        }

        bool ICollection.IsSynchronized {
            get { return IsSynchronized; }
        }

        object ICollection.SyncRoot {
            get { return SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
