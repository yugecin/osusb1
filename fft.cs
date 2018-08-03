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
