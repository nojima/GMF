using System;
using System.Collections.Generic;

namespace GeneralizedMaximumFlow {
    /// <summary>
    /// 優先度キュー
    /// 小さい要素から先に取り出す
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class PriorityQueue<T> where T : IComparable<T> {
        private List<T> heap;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PriorityQueue() {
            heap = new List<T>();
        }

        /// <summary>
        /// キューにobjを入れる
        /// </summary>
        /// <param name="obj"></param>
        public void Enqueue(T obj) {
            heap.Add(obj);
            SiftUp();
        }

        /// <summary>
        /// キューから値が最も小さいオブジェクトを削除して返す
        /// </summary>
        /// <returns></returns>
        public T Dequeue() {
            T result = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            SiftDown();
            return result;
        }

        /// <summary>
        /// キュー内の要素数を返す
        /// </summary>
        public int Count {
            get { return heap.Count; }
        }

        /// <summary>
        /// キュー内のオブジェクトを全て削除する
        /// </summary>
        public void Clear() {
            heap.Clear();
        }

        private void SiftUp() {
            int i = heap.Count - 1;
            while (true) {
                int j = (i - 1) >> 1;
                if (j < 0 || heap[i].CompareTo(heap[j]) >= 0) { break; }
                Swap(i, j);
            }
        }

        private void SiftDown() {
            int i = 0;
            while (true) {
                int j = 2 * i + 1;
                if (j >= heap.Count) { break; }
                if (j + 1 >= heap.Count) {
                    if (heap[i].CompareTo(heap[j]) > 0) { Swap(i, j); }
                    break;
                }
                if (heap[i].CompareTo(heap[j]) <= 0 && heap[i].CompareTo(heap[j + 1]) <= 0) {
                    break;
                }
                if (heap[j].CompareTo(heap[j + 1]) < 0) {
                    Swap(i, j);
                    i = j;
                } else {
                    Swap(i, j + 1);
                    i = j + 1;
                }
            }
        }

        private void Swap(int i, int j) {
            T t = heap[i];
            heap[i] = heap[j];
            heap[j] = t;
        }
    }
}
