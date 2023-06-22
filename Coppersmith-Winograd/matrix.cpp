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

Matrix Matrix::operator*(const Matrix &matrix)
{
	if (this->_columns != matrix._rows) {
		std::cout << "Multiplication is impossible";
	} 
	else {
		int d = this->_columns / 2;
		double *rowFactor = new double[this->_rows];
		double *columnFactor = new double[matrix._columns];

		for (int i = 0; i < this->_rows; i++) {
			rowFactor[i] = this->_data[i][0] * this->_data[i][1];
			for (int j = 1; j < d; j++) {
				rowFactor[i] += this->_data[i][2*j - 1] * this->_data[i][2*j];
			}
		}
		for (int i = 0; i < matrix._columns; i++) {
			columnFactor[i] = matrix._data[0][i] * matrix._data[1][i];
			for (int j = 1; j < d; j++) {
				columnFactor[i] += matrix._data[2*j - 1][i] * matrix._data[2*j][i];
			}
		}

		Matrix result(this->_rows, matrix._columns);

		for (int i = 0; i < this->_rows; i++) {
			for (int j = 0; j < matrix._columns; j++) {
				result._data[i][j] = -rowFactor[i] - columnFactor[j];
				for (int k = 0; i < d; k++) {
					result._data[i][j] += (this->_data[i][2*k - 1] + matrix._data[2*k][j]) * 
						(this->_data[i][2*k] + matrix._data[2*k - 1][j]);
				}
			}
		}
		if (2 * d != this->_columns) {
			for (int i = 1; i < this->_rows; i++) {
				for (int j = 1; j < matrix._columns; j++) {
					result._data[i][j] += this->_data[i][this->_columns] * 
						matrix._data[this->_columns][j];
				}
			}
		}

		return result;
	}
}
