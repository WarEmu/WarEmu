// Example program
#include <iostream>
#include <string>
#include <chrono>

struct Byte
{
    unsigned char mByte;
    
    Byte()
    {
        this->mByte = 0;    
    }
    
    Byte(unsigned char b)
    {
        this->mByte = b;    
    }
    
    friend std::ostream& 
    operator<<(std::ostream& out, const Byte& b)
    {
        return out << (int)b.mByte;
    }
};

unsigned long long
GetCurrentMS()
{
    return std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
}

//usage example:
//
//  {
//      TimeMeasure tm;
//      {
//          std::cout << "f(42) = " << fibonacci(44) << '\n';
//      }
//  }
//
struct TimeMeasure
{
    unsigned long long mStart;
    
    TimeMeasure()
    {
        this->mStart = this->GetCurrentMS();
    }
    
    ~TimeMeasure()
    {
        unsigned long long end = this->GetCurrentMS();
        std::cout << "TimeMeasure: " << end-this->mStart << " ms";
    }
    
    unsigned long long
    GetCurrentMS()
    {
        return std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
    }
};

unsigned long long 
fibonacci(unsigned int n)
{
    if (n < 2) return n;
    return fibonacci(n-1) + fibonacci(n-2);
}

int main()
{
    {
        TimeMeasure tm;
        {
            std::cout << "f(42) = " << fibonacci(44) << '\n';
        }
    }

    return 0;
}
