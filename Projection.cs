using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace osusb1 {
	class Projection {

		List<CAMDATA> cameras = new List<CAMDATA>();
		CAMDATA cm;
		public P3D campos;
	
		struct CAMDATA {
			public int time;
			public float x;
			public float y;
			public float z;
			public CMATRIX m;
		}

		struct CMATRIX {
			public float _11;
			public float _12;
			public float _13;
			public float _14;
			public float _21;
			public float _22;
			public float _23;
			public float _24;
			public float _31;
			public float _32;
			public float _33;
			public float _34;
			public float _41;
			public float _42;
			public float _43;
			public float _44;
		}

		public Projection() {
			using (StreamReader r = new StreamReader("camera.txt")) {
				foreach (string line in r.ReadToEnd().Split('\n')) {
					string[] tokens = line.Split(' ');
					if (tokens.Length != 20) {
						continue;
					}
					CAMDATA cd;
					CMATRIX cm;
					int i = 0;
					cd.time = int.Parse(tokens[i++]);
					cd.x = float.Parse(tokens[i++]);
					cd.y = float.Parse(tokens[i++]);
					cd.z = float.Parse(tokens[i++]);
					cm._11 = float.Parse(tokens[i++]);
					cm._12 = float.Parse(tokens[i++]);
					cm._13 = float.Parse(tokens[i++]);
					cm._14 = float.Parse(tokens[i++]);
					cm._21 = float.Parse(tokens[i++]);
					cm._22 = float.Parse(tokens[i++]);
					cm._23 = float.Parse(tokens[i++]);
					cm._24 = float.Parse(tokens[i++]);
					cm._31 = float.Parse(tokens[i++]);
					cm._32 = float.Parse(tokens[i++]);
					cm._33 = float.Parse(tokens[i++]);
					cm._34 = float.Parse(tokens[i++]);
					cm._41 = float.Parse(tokens[i++]);
					cm._42 = float.Parse(tokens[i++]);
					cm._43 = float.Parse(tokens[i++]);
					cm._44 = float.Parse(tokens[i++]);
					cd.m = cm;
					cameras.Add(cd);
				}
				this.cm = cameras[0];
			}
		}

		public void Update(int time) {
			this.cm = cameras[0];
			foreach (CAMDATA d in cameras) {
				if (d.time > time) {
					break;
				}
				this.cm = d;
			}
			campos.x = this.cm.x;
			campos.y = this.cm.y;
			campos.z = this.cm.z;
		}

		public P3D Project(P3D p) {
			P3D n;
			n.x = cm.m._41 + p.z * cm.m._31 + p.y * cm.m._21 + p.x * cm.m._11;
			n.y = cm.m._42 + p.z * cm.m._32 + p.y * cm.m._22 + p.x * cm.m._12;
			n.z = cm.m._43 + p.z * cm.m._33 + p.y * cm.m._23 + p.x * cm.m._13;
			float f = 1f / n.z;
			n.x *= f * 640f;
			n.y *= f * 480f;
			return n;
		}

	}
}
