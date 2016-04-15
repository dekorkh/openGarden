using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.IO;

namespace OpenGarden
{
    public class GameAudio
    {
        AudioContext AC;
        int[] audioBuffers;     //prsm. similar to vbo's but for audio (req. by call to AL.GenBuffers)

        public GameAudio()
        {
            //New audio context
            AC = new AudioContext();
            
            //Generate two buffers (kinda like VBO's)
            audioBuffers = AL.GenBuffers(2);

            
        }

        ~GameAudio()
        {
            for (int i = 0; i < audioBuffers.Length; i++)
                AL.DeleteBuffer(audioBuffers[i]);
            AC.Dispose();
        }

        public Sound CreateSoundSource(string filename)
        {
            //Generate a source
            int source = AL.GenSource();
            int channels, bits_per_sample, sample_rate; //Passed by ALBufferData call along with the sound byte data.
            //Load the actual wave form data and retrieve channels, beats/sample, & sample rate.
            var sound_data = LoadWave(
                File.Open(filename, FileMode.Open),
                out channels,
                out bits_per_sample,
                out sample_rate);
            //Chose the correct sound format based on # of channels & beats/sample.
            var sound_format =
                channels == 1 && bits_per_sample == 8 ? ALFormat.Mono8 :
                channels == 1 && bits_per_sample == 16 ? ALFormat.Mono16 :
                channels == 2 && bits_per_sample == 8 ? ALFormat.Stereo8 :
                channels == 2 && bits_per_sample == 16 ? ALFormat.Stereo16 :
                (ALFormat)0; // unknown
            //move the data to the buffer (will still need a source)
            AL.BufferData(audioBuffers[0], sound_format, sound_data, sound_data.Length, sample_rate);
            if (AL.GetError() != ALError.NoError)
            {
                // respond to load error etc.
                Console.WriteLine(AL.GetErrorString(AL.GetError()));
            }
            //associate source with the audio data in the buffer
            AL.Source(source, ALSourcei.Buffer, audioBuffers[0]);
            return new Sound(filename, source);
        }

        // Loads a wave/riff audio file.
        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            //Stream an open file handle.
            if (stream == null)
                throw new ArgumentNullException("stream");

            //Use a binary reader to traverse
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                //this seems useless
                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }
    }
}
