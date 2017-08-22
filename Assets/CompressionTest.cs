using System;
using System.Threading;
using UnityEngine;
using NSpeex;

public class CompressionTest 
{
    static SpeexEncoder encoder = new SpeexEncoder(BandMode.Narrow);
    static SpeexDecoder decoder = new SpeexDecoder(BandMode.Narrow);

    
    public static void Encode(float[] audioData, Action<byte[]> callback)
    {
        Thread t = new Thread(new ParameterizedThreadStart(DoEncode));
        t.Start(new EncodeState(audioData, callback));
    }

    
    private static void DoEncode(object encodeState)
    {
        EncodeState state = encodeState as EncodeState;

        short[] shortData = new short[state.Samples.Length];
        ToShortArray(state.Samples, shortData, 0, state.Samples.Length);
        byte[] compressedData = new byte[shortData.Length * 2];

        int sampleLen = shortData.Length - shortData.Length % encoder.FrameSize;
        int len = encoder.Encode(shortData, 0, sampleLen, compressedData, 0, compressedData.Length);

        Array.Resize<byte>(ref compressedData, len);

        state.Callback.Invoke(compressedData);
    }





    
    public static void Decode(byte[] compressedData, Action<float[]> callback)
    {
        Thread t = new Thread(new ParameterizedThreadStart(DoDecode));
        t.Start(new DecodeState(compressedData, callback));
    }

    
    private static void DoDecode(object decodeState)
    {
        DecodeState state = decodeState as DecodeState;

        short[] outData = new short[500000];
        int len = decoder.Decode(state.Datas, 0, state.Datas.Length, outData, 0, false);

        float[] pcmData = new float[len];
        ToFloatArray(outData, pcmData, pcmData.Length);

        state.Callback.Invoke(pcmData);
    }


    static void ToShortArray(float[] input, short[] output, int offset, int len)
    {
        for (int i = 0; i < len; ++i)
        {
            output[i] = (short)Mathf.Clamp((int)(input[i + offset] * 32767.0f), short.MinValue, short.MaxValue);
        }
    }

    static void ToFloatArray(short[] input, float[] output, int length)
    {
        for (int i = 0; i < length; ++i)
        {
            output[i] = input[i] / (float)short.MaxValue;
        }
    }
}

class EncodeState
{
    public float[] Samples;
    public Action<byte[]> Callback;

    public EncodeState(float[] samples, Action<byte[]> callback)
    {
        Samples = samples;
        Callback = callback;
    }
}

class DecodeState
{
    public byte[] Datas;
    public Action<float[]> Callback;

    public DecodeState(byte[] datas, Action<float[]> callback)
    {
        Datas = datas;
        Callback = callback;
    }
}
