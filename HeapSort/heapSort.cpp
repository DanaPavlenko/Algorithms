/*
 * @file   heapSort.cpp
 * @author Dana
 *
 */

#include <iostream>

using namespace std;

void heapify(int pos, int n, int *arr)
{
	while (2 * pos + 1 < n) {
		int t = 2 * pos + 1;
		if (2 * pos + 2 < n && arr[2 * pos + 2] >= arr[t]) {
			t = 2 * pos + 2;
		}
		if (arr[pos] < arr[t]) {
			swap(arr[pos], arr[t]);
			pos = t;
		}
		else {
			break;
		}
	}
}

void heap_make(int n, int *arr)
{
	for (int i = n - 1; i >= 0; i--) {
		heapify(i, n, arr);
	}
}

void heap_sort(int n, int *arr)
{
	heap_make(n, arr);
	cout << "Heap: ";
	for (int i = 0; i < n; i++) {
		cout << arr[i] << " ";
	}
	cout << endl;
	cout << "Sort: " << endl;

	int k = n;

	while (n > 0) {
		swap(arr[0], arr[n - 1]);
		n--;
		heapify(0, n, arr);

		for (int i = 0; i < k; i++) {
			cout << arr[i] << " ";
		}
		cout << endl;
	}
}
