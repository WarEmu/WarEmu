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
    enum UNIT
    {
        INVALIDE = 0,
        
        MILLIESECONDS,
        SECONDS,
        
        COUNT 
    };
    unsigned long long mStart;
    unsigned long long mEnd;
    UNIT mResultUnit;
    
    TimeMeasure( UNIT resultUnit = MILLIESECONDS )
    {
        this->Start();
        this->mResultUnit = resultUnit;
    }
    
    ~TimeMeasure()
    {
        this->End();
        this->PrintResult();
    }
    
    unsigned long long
    Start()
    {
        this->mStart = this->GetCurrentMS();
        return this->mStart;
    }
    
    unsigned long long
    End()
    {
        this->mEnd = this->GetCurrentMS();
        return this->mEnd;
    }
    
    void
    PrintResult()
    {
        unsigned long long resultInMS = this->mEnd-this->mStart;
        switch(this->mResultUnit)
        {
            case MILLIESECONDS:
            {
                std::cout << "TimeMeasure: " << resultInMS << " ms" << std::endl;
            }
            break;
            
            case SECONDS:
            {
                std::cout << "TimeMeasure: " << resultInMS*0.001f << " s" << std::endl;
            }
            break;
            
            default:
            {
                std::cout << "TimeMeasure: Unhandeled mResultUnit" << std::endl;
            }
            break;
        }
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
        TimeMeasure tm(TimeMeasure::SECONDS);
        {
            std::cout << "f = " << fibonacci(10) << std::endl;
        }
    }
    
    {
        TimeMeasure tm;
        {
            std::cout << "f = " << fibonacci(35) << std::endl;
        }
    }

    return 0;
}
