using System;

namespace ExtractFromScan
{
  static class MinimumFinder
  {
    public static float FindMinimum(Func<float, float> func, float start, float end, float precision)
    {
      const int N = 10;
      float length = end - start;
      float step = length/N;
      var results = new float[N];
      float min = float.MaxValue;
      int minIndex = -1;
      for(int i = 0; i < N; i++)
      {
        float x = start + step*i;
        float result = func(x);
        results[i] = result;
        if (result < min)
        {
          min = result;
          minIndex = i;
        }
      }

      float minX = start + step*minIndex;
      if (step <= precision) return minX;

      int prevIndex = minIndex - 1;
      int nextIndex = minIndex + 1;
      float prevX = start + step*prevIndex;
      float nextX = start + step*nextIndex;
      float prevResult = prevIndex >= 0 ? func(prevX) : float.MaxValue;
      float nextResult = nextIndex < N ? func(nextX) : float.MaxValue;
      if (prevResult < nextResult)
        return FindMinimum(func, prevX, minX, precision);
      else
        return FindMinimum(func, minX, nextX, precision);
    }
  }
}