using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace osusb1 {
partial class all {
class FFT {

	public const int FREQS = 15;

	List<FRAME> frames;
	public FRAME frame;
	public FRAME smoothframe;

	public class FRAME {
		public int time;
		public float[] values;
	}

	public FFT() {
		frames = new List<FRAME>();
		float maxvol = 0f;
		using (StreamReader r = new StreamReader("fft.txt")) {
			string line;
			while ((line = r.ReadLine()) != null) {
				if (line.Length == 0) {
					continue;
				}

				string[] p = line.Split(' ');
				FRAME f = new FRAME();
				f.time = int.Parse(p[0]);
				f.values = new float[FREQS];
				for (int i = 0; i < FREQS; i++) {
					f.values[i] = float.Parse(p[1 + i]);
					maxvol = max(maxvol, f.values[i]);
				}
				frames.Add(f);
			}
		}
		foreach (FRAME f in frames) {
			for (int i = 0; i < FREQS; i++) {
				f.values[i] /= maxvol;
			}
		}
		Console.WriteLine("{0} audio frames, maxvol {1}", frames.Count, maxvol);
	}

	public void Update(int time) {
		int idx = 0;
		for (; idx < frames.Count; idx++) {
			frame = frames[idx];
			if (frames[idx].time > time) {
				break;
			}
		}
		const float FALLOFF = .002f;
		const float FALLUP = .009f;
		smoothframe = new FRAME();
		smoothframe.time = time;
		smoothframe.values = new float[FREQS];
		for (int j = 0; j < FREQS; j++) {
			float value = 0f;
			for (int i = max(0, idx - 15); i <= idx; i++) {
				FRAME pf = frames[i];
				value = max(value, pf.values[j] - (frame.time - pf.time) * FALLOFF);
			}
			for (int i = idx + 1; i < min(frames.Count - 1, idx + 15); i++) {
				FRAME pf = frames[i];
				value = max(value, pf.values[j] - (pf.time - frame.time) * FALLUP);
			}
			smoothframe.values[j] = value;
		}
	}
	
}
}
}
