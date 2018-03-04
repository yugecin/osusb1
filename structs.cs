﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace osusb1 {
	struct P3D {
		public float x, y, z, dist;
		public P3D(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.dist = 0f;
		}
	}
	struct DATA {
		public int time;
		public int a;
		public DATA(int time, int a) {
			this.time = time;
			this.a = a;
		}
	}
	struct DATAf {
		public int time;
		public float a;
		public DATAf(int time, float a) {
			this.time = time;
			this.a = a;
		}
	}
	struct BIDATA {
		public int time;
		public int a;
		public int b;
		public BIDATA(int time, int a, int b) {
			this.time = time;
			this.a = a;
			this.b = b;
		}
	}
	struct BIDATAf {
		public int time;
		public float a;
		public float b;
		public BIDATAf(int time, float a, float b) {
			this.time = time;
			this.a = a;
			this.b = b;
		}
	}
	struct SCENE {
		public int starttime;
		public int endtime;
		public int time;
		public int reltime;
		public float progress;
		public Projection projection;
		public Graphics g;
		public SCENE(int starttime, int endtime, int time, Projection projection, Graphics g) {
			this.starttime = starttime;
			this.endtime = endtime;
			this.time = time;
			this.reltime = this.time - this.starttime;
			this.progress = reltime / (float) (this.endtime - this.starttime);
			this.projection = projection;
			this.g = g;
		}
	}
}
