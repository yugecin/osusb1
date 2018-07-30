using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace osusb1 {
partial class all {

	public static vec3 campos = v3();

	class Projection {

		List<CAMDATA> cameras = new List<CAMDATA>();
		CAMDATA cd;
	
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
				this.cd = cameras[0];
			}
		}

		public void Update(int time) {
			this.cd = cameras[0];
			foreach (CAMDATA d in cameras) {
				if (d.time > time) {
					break;
				}
				this.cd = d;
			}
			campos.x = this.cd.x;
			campos.y = this.cd.y;
			campos.z = this.cd.z;
		}

		public vec4 Project(vec3 p) {
			vec4 n = v4();
			n.x = cd.m._41 + p.z * cd.m._31 + p.y * cd.m._21 + p.x * cd.m._11;
			n.y = cd.m._42 + p.z * cd.m._32 + p.y * cd.m._22 + p.x * cd.m._12;
			n.z = cd.m._43 + p.z * cd.m._33 + p.y * cd.m._23 + p.x * cd.m._13;
			n.w = distance(p, campos);
			if (n.z == 0) {
				n.z = -1f;
			}
			float f = 1f / n.z;
			n.x *= f * 640f;
			n.y *= f * 448f;
			return n;
		}

	}

	static bool isOnScreen(vec2 pos) {
		int x = (int) pos.x;
		int y = (int) pos.y;
		return 0 <= x && x < 640 && 0 <= y && y < 480;
	}

	static bool isOnScreen(vec2 pos, float size) {
		float th = (int) (size / 2f);
		int x = (int) pos.x;
		int y = (int) pos.y;
		return -th <= x && x < 640 + th && -th <= y && y < 480 + th;
	}

	static bool isOnScreen(vec2 pos, vec2 size) {
		vec2 th = size / 2f;
		int thx = (int) th.x;
		int thy = (int) th.y;
		int x = (int) pos.x;
		int y = (int) pos.y;
		return -thx <= x && x < 640 + thx && -thy <= y && y < 480 + thy;
	}

}
}
