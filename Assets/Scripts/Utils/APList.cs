using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PooledList<T> : IList<T>, IDisposable {
    private T[] _items;
    private int _count;
    private static readonly ArrayPool<T> _pool = ArrayPool<T>.Shared;

    public int Count => _count;
    public bool IsReadOnly => false;
    public T this[int index] {
        get => _items[index];
        set => _items[index] = value;
    }

    public PooledList(int capacity = 10) {
        _items = _pool.Rent(capacity);
        _count = 0;
    }

    public void Add(T item) {
        if (_count == _items.Length) {
            Resize(_items.Length * 2);
        }
        _items[_count++] = item;
    }

    public void Clear() {
        Array.Clear(_items, 0, _count);
        _pool.Return(_items, clearArray: true);
        _items = _pool.Rent(10); // 초기 크기로 재할당
        _count = 0;
    }

    public bool Contains(T item) => _items.Take(_count).Contains(item);

    public void CopyTo(T[] array, int arrayIndex) {
        Array.Copy(_items, 0, array, arrayIndex, _count);
    }

    public bool Remove(T item) {
        int index = IndexOf(item);
        if (index < 0) return false;

        _count--;
        Array.Copy(_items, index + 1, _items, index, _count - index);
        _items[_count] = default;
        return true;
    }

    public int IndexOf(T item) {
        for (int i = 0; i < _count; i++) {
            if (EqualityComparer<T>.Default.Equals(_items[i], item)) {
                return i;
            }
        }
        return -1;
    }

    public void Insert(int index, T item) {
        if (_count == _items.Length) {
            Resize(_items.Length * 2);
        }
        Array.Copy(_items, index, _items, index + 1, _count - index);
        _items[index] = item;
        _count++;
    }

    public void RemoveAt(int index) {
        if (index < 0 || index >= _count) return;
        _count--;
        Array.Copy(_items, index + 1, _items, index, _count - index);
        _items[_count] = default;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator() {
        for (int i = 0; i < _count; i++) {
            yield return _items[i];
        }
    }

    private void Resize(int newSize) {
        T[] newItems = _pool.Rent(newSize);
        Array.Copy(_items, 0, newItems, 0, _count);
        _pool.Return(_items, clearArray: true);
        _items = newItems;
    }

    public void Dispose() {
        _pool.Return(_items, clearArray: true);
        _items = null;
    }
}