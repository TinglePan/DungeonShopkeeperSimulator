shader_type canvas_item;
uniform sampler2D mapAtlas;
uniform sampler2D mapFgColorAtlas;
uniform sampler2D mapBgColorAtlas;
uniform vec2 mapSize;
uniform sampler2D glyphAtlas;
uniform vec2 glyphAtlasSize;
uniform vec2 tileSize;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	float x = UV.x * mapSize.x;
	float col = floor(x);
	float offsetX = x - col;
	float y = UV.y * mapSize.y;
	float row = floor(y);
	float offsetY = y - row;
	ivec2 coord = ivec2(vec2(col, row));
	vec4 tileData = texelFetch(mapAtlas, coord, 0);
	float glyphId = tileData.r * 255.0;
	float test = mod(glyphId, glyphAtlasSize.x);
	float tileX = (mod(glyphId, glyphAtlasSize.x) + offsetX) * tileSize.x;
	float tileY = (float(int(glyphId) / int(glyphAtlasSize.x)) + offsetY) * tileSize.y;
	vec4 color = texelFetch(glyphAtlas, ivec2(vec2(tileX, tileY)), 0);
	float factor = step(0.5, color.a);
	vec4 fgColor = texelFetch(mapFgColorAtlas, coord, 0);
	vec4 bgColor = texelFetch(mapBgColorAtlas, coord, 0);
	color = fgColor * factor + bgColor * (1.0f - factor);
	//color = vec4(factor, 0, 0, 1.0);
	//vec4 color = vec4(glyphId, 0, 0, 255);
	//vec4 color = vec4(tileX / tileSize.x / glyphAtlasSize.x, 0, 0, 1.0);
	COLOR = color;
}

void light() {
	// Called for every pixel for every light affecting the CanvasItem.
}

