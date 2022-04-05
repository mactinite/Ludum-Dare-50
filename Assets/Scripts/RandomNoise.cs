using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNoise
{
    // The base bit-noise constants were crafted to have distinctive and interesting
    // bits, and have so far produced excellent experimental test results.
    static uint NOISE1 = 0xb5297a4d;  // 0b0110'1000'1110'0011'0001'1101'1010'0100
    static uint NOISE2 = 0x68e31da4; // 0b1011'0101'0010'1001'0111'1010'0100'1101
    static uint NOISE3 = 0x1b56c4e9; // 0b0001'1011'0101'0110'1100'0100'1110'1001
    // returns a float between 0 and 1 based on squirrel3 noise function
    // introduced by Squirrel Eiserloh at 'Math for Game Programmers: Noise-based RNG', GDC17.
    public static float Get2DNoiseFloat(uint x, uint y, uint seed = 0)
    {
        uint val = Get2DNoiseUInt(x, y, seed);
        float newVal = (val / (float)uint.MaxValue);
        return newVal;
    }

    // returns a float between 0 and 1 based on squirrel3 noise function
    // introduced by Squirrel Eiserloh at 'Math for Game Programmers: Noise-based RNG', GDC17.
    public static float Get1DNoiseFloat(uint pos, uint seed = 0)
    {
        return Get1DNoiseUInt(pos, seed) / uint.MaxValue;
    }

    // returns a uint between based on squirrel3 noise function
    // introduced by Squirrel Eiserloh at 'Math for Game Programmers: Noise-based RNG', GDC17.
    public static uint Get2DNoiseUInt(uint x, uint y, uint seed = 0)
    {
        uint PRIME = 198491317; // large prime number with non-boring digits
        return Get1DNoiseUInt(x + (PRIME * y), seed);
    }

    // returns a float between 0 and 1 based on squirrel3 noise function
    // introduced by Squirrel Eiserloh at 'Math for Game Programmers: Noise-based RNG', GDC17.
    public static uint Get1DNoiseUInt(uint pos, uint seed = 0)
    {
        uint mangled = pos;
        mangled *= NOISE1;
        mangled += seed;
        mangled ^= mangled >> 8;
        mangled += NOISE2;
        mangled ^= mangled << 8;
        mangled *= NOISE3;
        mangled ^= mangled >> 8;
        
        return mangled;
    }
}
