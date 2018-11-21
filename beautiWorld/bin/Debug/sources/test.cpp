#include <iostream>
using namespace std;

int main(int argc, char const *argv[])
{
	double sum = 0;
	int i=1;
	int max;
	cin>>max;
	while(sum<max)
	{
		sum+=1.0/i;
		i++;
	}
	cout<<i-1;
	return 0;
}