/*
 * @file   matrix.h
 * @author Dana
 *
 */

#ifndef MATRIX_H
#define MATRIX_H    

#include <iostream>
#include <iomanip>
#include <ctime>

using namespace std;

class Matrix
{
private:
	int **_data;
	int _rows, _columns;

public:
	Matrix(int, int);
	Matrix(const Matrix&);
	~Matrix();

	void Set();
	void Show();
	void Random();
	void GaussElimination();

	Matrix operator*(const Matrix&);
};

#endif	/* MATRIX_H */
