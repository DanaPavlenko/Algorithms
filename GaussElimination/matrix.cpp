/*
 * @file   matrix.cpp
 * @author Dana
 *
 */

#include "matrix.h"

Matrix::Matrix(int n, int m)
{
	_rows = n;
	_columns = m;
	_data = new int*[_rows];
	for (int i = 0; i < _rows; i++) {
		_data[i] = new int[_columns];
		for (int j = 0; j < _columns; j++) {
			_data[i][j] = 0;
		}
	}
}

Matrix::Matrix(const Matrix& matrix)
{
	_rows = matrix._rows;
	_columns = matrix._columns;
	_data = new int *[_rows];
	for (int i = 0; i < _rows; i++) {
		_data[i] = new int[_columns];
		for (int j = 0; j < _columns; j++) {
			_data[i][j] = matrix._data[i][j];
		}
	}
}

Matrix::~Matrix()
{
	for (int i = 0; i < _rows; i++) {
		delete[] _data[i];
	}
	delete[] _data;
}

void Matrix::Set()
{
	for (int i = 0; i < _rows; i++) {
		for (int j = 0; j < _columns; j++) {
			cin >> _data[i][j];
		}
	}
}

void Matrix::Show()
{
	for (int i = 0; i < _rows; i++) {
		for (int j = 0; j < _columns; j++) {
			cout << setw(2) << _data[i][j] << " ";
		}
		cout << std::endl;
	}
}

void Matrix::Random()
{
	srand(time(0));
	for (int i = 0; i < _rows; i++) {
		for (int j = 0; j < _columns; j++) {
			_data[i][j] = rand() % 10;
		}
	}
}

void Matrix::GaussElimination()
{
	if (_rows < _columns - 1) {
		std::cout << "Solution is impossible";
	}
	else {
		double *result = new double[_rows],
			   **buffer = new double *[_rows], temp;
		for (int i = 0; i < _rows; i++) {
			buffer[i] = new double[_columns];
			for (int j = 0; j < _columns; j++) {
				buffer[i][j] = (double)_data[i][j];
			}
		}
		int k;
		for (int i = 0; i < _rows; i++) {
			temp = buffer[i][i];
			for (int j = _rows; j >= i; j--) {
				buffer[i][j] /= temp;
			}
			for (int j = i + 1; j < _rows; j++) {
				temp = buffer[j][i];
				for (k = _rows; k >= i; k--) {
					buffer[j][k] -= temp * buffer[i][k];
				}
			}
		}
		result[_rows - 1] = buffer[_rows - 1][_rows];
		for (int i = _rows - 2; i >= 0; i--) {
			result[i] = buffer[i][_rows];
			for (int j = i + 1; j < _rows; j++) {
				result[i] -= buffer[i][j] * result[j];
			}
		}
		cout << endl
			 << "Result:" << endl;
		for (int i = 0; i < _rows; i++) {
			cout << result[i] << " ";
		}
		cout << endl;
	}
}

Matrix Matrix::operator*(const Matrix &matrix)
{
	if (this->_columns != matrix._rows) {
		std::cout << "Multiplication is impossible";
	}
	else {
		Matrix result(this->_rows, matrix._columns);
		for (int i = 0; i < result._rows; i++)
		{
			for (int j = 0; j < result._columns; j++) {
				int temp = 0;
				for (int k = 0; k < this->_columns; k++) {
					temp += this->_data[i][k] * matrix._data[k][j];
				}
				result._data[i][j] = temp;
			}
		}
		return result;
	}
}
