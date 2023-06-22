/*
 * @file   FeistelCipher.cpp
 * @author Dana
 *
 */

#include <stdio.h>
#include <string.h>
#include <stdlib.h>

int rol(int a, int n)
{
	int t1, t2;
	n = n % (sizeof(a) * 8);
	t1 = a << n;
	t2 = a >> (sizeof(a) * 8 - n);
	return t1 | t2;
};

int f(int subblock, int key)
{
	return rol(subblock, key);
};

void encryption(int* left, int* right, int rounds, int* key)
{
	int i, temp;
	for (i = 0; i < rounds; i++) {
		temp = *right ^ f(*left, key[i]);
		*right = *left;
		*left = temp;
	}
};

void decryption(int* left, int* right, int rounds, int* key)
{
	int i, temp;
	for (i = rounds - 1; i >= 0; i--) {
		temp = *left ^ f(*right, key[i]);
		*left = *right;
		*right = temp;
	}
};

int main(int argc, char* argv[])
{
	if (argc != 3) {
		printf("UNKNOWN FORMAT");
		exit(0);
	}

	int a[7] = {1, 4, 8, 8, 3, 2, 2};
	int *keys = a;

	FILE *source;
	source = fopen(argv[2], "rb");

	if (source == NULL) {
		printf("FILE NOT EXIST");
		exit(1);
	}

	long long block;
	int blockL;
	int blockR;

	char* name = (char*)calloc(strlen(argv[2]) + 4, sizeof(char));
	char* dot = strrchr(argv[2], '.');
	strncpy(name, argv[2], (int)(dot - argv[2]));

	if (!strcmp(argv[1], "encryption")) {
		strcat(name, "_enc");
		strcat(name, dot);
		FILE *crypted;
		crypted = fopen(name, "wb");

		while (!feof(source)) {
			fread(&blockL, 4, 1, source);
			fread(&blockR, 4, 1, source);
			encryption(&blockL, &blockR, 7, keys);
			fwrite(&blockL, 4, 1, crypted);
			fwrite(&blockR, 4, 1, crypted);
		}

		printf("ENCRYPTED\n");
		fclose(crypted);
	}
	else if (!strcmp(argv[1], "decryption")) {
		strcat(name, "_dec");
		strcat(name, dot);
		FILE *decrypted;
		decrypted = fopen(name, "wb");

		while (!feof(source)) {
			fread(&blockL, 4, 1, source);
			fread(&blockR, 4, 1, source);
			decryption(&blockL, &blockR, 7, keys);
			fwrite(&blockL, 4, 1, decrypted);
			fwrite(&blockR, 4, 1, decrypted);
		}

		fclose(decrypted);
		printf("DECRYPTED\n");
	}
	else {
		printf("UNKNOWN COMAND: %s", argv[1]);
	}

	fclose(source);
	return 1;
};
