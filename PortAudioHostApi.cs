using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FlexASIOGUI
{
    static class PortAudioHostApi
    {
        public static int Count => Native.Pa_GetHostApiCount();

        public static string GetName(int hostApi)
        {
            var infoPtr = Native.Pa_GetHostApiInfo(hostApi);
            if (infoPtr == IntPtr.Zero)
                return "";

            var info = Marshal.PtrToStructure<HostApiInfo>(infoPtr);
            return ReadUtf8(info.name);
        }

        private static string ReadUtf8(IntPtr value)
        {
            if (value == IntPtr.Zero)
                return "";

            int length = 0;
            while (Marshal.ReadByte(value, length) != 0)
                length++;

            if (length == 0)
                return "";

            var buffer = new byte[length];
            Marshal.Copy(value, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HostApiInfo
        {
            public int structVersion;
            public int type;
            public IntPtr name;
            public int deviceCount;
            public int defaultInputDevice;
            public int defaultOutputDevice;
        }

        private static class Native
        {
            [DllImport("portaudio")]
            public static extern int Pa_GetHostApiCount();

            [DllImport("portaudio")]
            public static extern IntPtr Pa_GetHostApiInfo(int hostApi);
        }
    }
}
