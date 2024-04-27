#include <stdio.h>
#include <stdlib.h>


int main()
{
	const char * str = "???";
	char c = str[0];
	printf("\n");

	if (c >= 65 && c <= 90) {
	printf("high\n");
	} else if (c >= 97 && c <=122) {
	printf("low\n");
	} else {
	printf("other\n");
	}

	return 0;
}