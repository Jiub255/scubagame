shader_type canvas_item;

uniform float rate = 0.1;

varying vec2 v;

void vertex() {
	// Called for every vertex the material is visible on.
	float pi = 3.141592653589793;
	VERTEX *= .5 + (sin(fract(TIME * rate) * pi * 2.) + 1.) /4.;

	v = VERTEX;
}

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR = texture(TEXTURE, UV);
	if (length(v) > 200.)
		COLOR.rg = UV.xy;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
