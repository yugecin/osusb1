using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace osusb1 {
partial class all {
class FFT {

	const int IFREQS = 40;
	const int OFREQS = 8;
	const int FREQI = IFREQS / OFREQS;

	List<FRAME> frames;
	public FRAME frame;

	public class FRAME {
		public int time;
		public int maxvol;
		public float[] values;
	}

	public FFT() {
		Debug.Assert(FREQI * OFREQS == IFREQS);
		frames = new List<FRAME>();
		int maxvol = 0;
		using (StreamReader r = new StreamReader("fft.txt")) {
			string line;
			FRAME currentframe = null;
			int freqcount = 0;
			float freqsum = 0f;
			while ((line = r.ReadLine()) != null) {
				if (line.Length == 0) {
					continue;
				}

				if (line[0] == 'M') {
					string[] parts = line.Split(':');
					currentframe = new FRAME();
					currentframe.maxvol = sqrt(int.Parse(parts[1].Split('\t')[0].Trim()));
					currentframe.time = int.Parse(parts[2].Trim().Replace(",", ""));
					currentframe.values = new float[OFREQS];
					if (currentframe.maxvol > maxvol) {
						maxvol = currentframe.maxvol;
					}
					freqsum = 0f;
					freqcount = 0;
					continue;
				}

				if (line[0] == 'f') {
					string[] parts = line.Split(':');
					freqsum += float.Parse(parts[2].Replace(',', '.'));
					freqcount++;
					if (freqcount % FREQI == 0) {
						currentframe.values[(freqcount / FREQI) - 1] = freqsum / FREQI;
					}
					if (freqcount == IFREQS) {
						float max = 0f;
						foreach (float v in currentframe.values) {
							if (v > max) {
								max = v;
							}
						}
						for (int i = 0; i < OFREQS; i++) {
							currentframe.values[i] /= max;
						}
						if (currentframe.maxvol > 20) {
							frames.Add(currentframe);
						}
					}
				}
			}
		}
		foreach (FRAME f in frames) {
			for (int i = 0; i < OFREQS; i++) {
				f.values[i] *= f.maxvol / (float) maxvol;
			}
		}
		Console.WriteLine("{0} audio frames, maxvol {1}", frames.Count, maxvol);
	}

	public void Update(int time) {
		this.frame = frames[0];
		foreach (FRAME f in frames) {
			this.frame = f;
			if (f.time > time) {
				break;
			}
		}
	}
	
}
}
}
