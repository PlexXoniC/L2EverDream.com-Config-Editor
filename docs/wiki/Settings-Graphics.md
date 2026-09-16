# Graphics settings

Resolution, detail, draw distance and effects in the game client.

**40 settings** in the **Graphics** category of the Client tab. [All categories](Settings-Reference) · [How to read this page](Settings-Reference#how-to-read-these-pages)

## Display

### Resolution width (GamePlayViewportX)

`Option.ini › [Video] › GamePlayViewportX` · number · px

Width of the game in pixels.

- **Default:** `1024`
- **Allowed:** 640 – 7680 px
- **Needs:** [Start in full screen](Settings-Graphics#start-in-full-screen-startupfullscreen) to be On — Full-screen resolution; the windowed size is set by the windowed mode width and height.

### Resolution height (GamePlayViewportY)

`Option.ini › [Video] › GamePlayViewportY` · number · px

Height of the game in pixels.

- **Default:** `768`
- **Allowed:** 480 – 4320 px
- **Needs:** [Start in full screen](Settings-Graphics#start-in-full-screen-startupfullscreen) to be On — Full-screen resolution; the windowed size is set by the windowed mode width and height.

### Start in full screen (StartupFullScreen)

`Option.ini › [Video] › StartupFullScreen` · on/off

- **Default:** On
- **Allowed:** On or Off
- **Controls:** [Resolution width](Settings-Graphics#resolution-width-gameplayviewportx) — it only works while this is On
- **Controls:** [Resolution height](Settings-Graphics#resolution-height-gameplayviewporty) — it only works while this is On
- **Controls:** [Windowed mode width](Settings-Graphics#windowed-mode-width-windowedviewportx) — it only works while this is Off
- **Controls:** [Windowed mode height](Settings-Graphics#windowed-mode-height-windowedviewporty) — it only works while this is Off

### Refresh rate (RefreshRate)

`Option.ini › [Video] › RefreshRate` · number · Hz

Should match your monitor.

- **Default:** `60`
- **Allowed:** 30 – 360 Hz

### Color depth (ColorBits)

`Option.ini › [Video] › ColorBits` · choice

- **Default:** `32` (32-bit)
- **Allowed:** one of `16` (16-bit), `32` (32-bit)

### Brightness (gamma) (Gamma)

`Option.ini › [Video] › Gamma` · slider

- **Default:** `0.5`
- **Allowed:** 0 – 1

### Force aspect ratio (ForceAspectRatio)

`l2.ini › [URL] › ForceAspectRatio` · on/off · *Advanced*

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Aspect ratio](Settings-Graphics#aspect-ratio-aspectratio) — it only works while this is On

### Aspect ratio (AspectRatio)

`l2.ini › [URL] › AspectRatio` · number · *Advanced*

1.334 = 4:3, 1.778 = 16:9.

- **Default:** `1.334`
- **Allowed:** 1 – 3
- **Needs:** [Force aspect ratio](Settings-Graphics#force-aspect-ratio-forceaspectratio) to be On — The aspect ratio is not being forced.

## Detail & effects

### Texture detail (TextureDetail)

`Option.ini › [Video] › TextureDetail` · choice

Same as the in-game Texture Detail option. If unsure which number is which, change it in game and compare.

- **Default:** `0` (0)
- **Allowed:** one of `0` (0), `1` (1), `2` (2)

### Character model detail (ModelDetail)

`Option.ini › [Video] › ModelDetail` · choice

Same as the in-game Model Detail option.

- **Default:** `0` (0)
- **Allowed:** one of `0` (0), `1` (1), `2` (2)

### Trilinear texture filtering (UseTrilinear)

`Option.ini › [Video] › UseTrilinear` · on/off

- **Default:** On
- **Allowed:** On or Off

### Anti-aliasing (AntiAliasing)

`Option.ini › [Video] › AntiAliasing` · choice

- **Default:** `0` (Off)
- **Allowed:** one of `0` (Off), `1` (On)

### Character shadows (PawnShadow)

`Option.ini › [Video] › PawnShadow` · on/off

- **Default:** On
- **Allowed:** On or Off

### Grass and decorations (RenderDeco)

`Option.ini › [Video] › RenderDeco` · on/off

- **Default:** On
- **Allowed:** On or Off

### Weather effects (WeatherEffect)

`Option.ini › [Video] › WeatherEffect` · choice

- **Default:** `0` (Off)
- **Allowed:** one of `0` (Off), `1` (On)

### Post-processing effects (PostProc)

`Option.ini › [Video] › PostProc` · choice

- **Default:** `0` (Off)
- **Allowed:** one of `0` (Off), `1` (On)

### Animate characters on the graphics card (GPUAnimation)

`Option.ini › [Video] › GPUAnimation` · on/off

- **Default:** On
- **Allowed:** On or Off

### Skip animations (SkipAnim)

`Option.ini › [Video] › SkipAnim` · choice

Lower-quality animations for more speed.

- **Default:** `0` (Off)
- **Allowed:** one of `0` (Off), `1` (On)

### Lower detail to keep the frame rate up (IsKeepMinFrameRate)

`Option.ini › [Video] › IsKeepMinFrameRate` · on/off

- **Default:** On
- **Allowed:** On or Off

### Water reflections (IsUseEffect)

`Option.ini › [L2WaterEffect] › IsUseEffect` · on/off

- **Default:** Off
- **Allowed:** On or Off
- **Controls:** [Water reflection quality](Settings-Graphics#water-reflection-quality-effecttype) — it only works while this is On

### Water reflection quality (EffectType)

`Option.ini › [L2WaterEffect] › EffectType` · choice · *Advanced*

- **Default:** `0` (0)
- **Allowed:** one of `0` (0), `1` (1), `2` (2)
- **Needs:** [Water reflections](Settings-Graphics#water-reflections-isuseeffect) to be On — Water reflections are off.

### Engine cache size (CacheSizeMegs)

`l2.ini › [Engine.GameEngine] › CacheSizeMegs` · number · MB · *Advanced*

- **Default:** `32`
- **Allowed:** 16 – 512 MB

## Draw distance

### Limit how many characters are drawn (RenderActorLimited)

`Option.ini › [Video] › RenderActorLimited` · number

Same as the in-game character limit option. Higher numbers draw more characters; useful in crowded towns full of sims.

- **Default:** `6`
- **Allowed:** 0 – 10

### Character draw distance (in-game option) (PawnClippingRange)

`Option.ini › [Video] › PawnClippingRange` · choice

- **Default:** `0` (0)
- **Allowed:** one of `0` (0), `1` (1), `2` (2), `3` (3), `4` (4)

### Terrain draw distance (in-game option) (TerrainClippingRange)

`Option.ini › [Video] › TerrainClippingRange` · choice

- **Default:** `0` (0)
- **Allowed:** one of `0` (0), `1` (1), `2` (2), `3` (3), `4` (4)

### Terrain distance (Terrain)

`Option.ini › [ClippingRange] › Terrain` · number · *Advanced*

- **Default:** `4`
- **Allowed:** 0 – 10

### Object distance (Actor)

`Option.ini › [ClippingRange] › Actor` · number · *Advanced*

- **Default:** `4`
- **Allowed:** 0 – 10

### Building distance (StaticMesh)

`Option.ini › [ClippingRange] › StaticMesh` · number · *Advanced*

- **Default:** `2`
- **Allowed:** 0 – 10

### Building detail distance (StaticMeshLod)

`Option.ini › [ClippingRange] › StaticMeshLod` · number · *Advanced*

- **Default:** `4`
- **Allowed:** 0 – 10

### Character distance (Pawn)

`Option.ini › [ClippingRange] › Pawn` · number · *Advanced*

- **Default:** `1`
- **Allowed:** 0 – 10

### Engine: farthest character distance (PawnMax)

`l2.ini › [ClippingRange] › PawnMax` · number · *Advanced*

- **Default:** `3`
- **Allowed:** 0 – 20

### Engine: nearest character distance (PawnMin)

`l2.ini › [ClippingRange] › PawnMin` · number · *Advanced*

- **Default:** `1.5`
- **Allowed:** 0 – 20

### Engine: terrain distance (Terrain)

`l2.ini › [ClippingRange] › Terrain` · number · *Advanced*

- **Default:** `8`
- **Allowed:** 0 – 20

### Engine: building distance (StaticMesh)

`l2.ini › [ClippingRange] › StaticMesh` · number · *Advanced*

- **Default:** `4`
- **Allowed:** 0 – 20

### Engine: object distance (Actor)

`l2.ini › [ClippingRange] › Actor` · number · *Advanced*

- **Default:** `4`
- **Allowed:** 0 – 20

## Window

### Colored mouse cursor (UseColorCursor)

`Option.ini › [Video] › UseColorCursor` · on/off

- **Default:** On
- **Allowed:** On or Off

### Windowed mode width (WindowedViewportX)

`l2.ini › [WinDrv.WindowsClient] › WindowedViewportX` · number · px

- **Default:** `640`
- **Allowed:** 640 – 7680 px
- **Needs:** [Start in full screen](Settings-Graphics#start-in-full-screen-startupfullscreen) to be Off — Only used when the game starts in a window.

### Windowed mode height (WindowedViewportY)

`l2.ini › [WinDrv.WindowsClient] › WindowedViewportY` · number · px

- **Default:** `480`
- **Allowed:** 480 – 4320 px
- **Needs:** [Start in full screen](Settings-Graphics#start-in-full-screen-startupfullscreen) to be Off — Only used when the game starts in a window.

### Full screen width (engine) (FullscreenViewportX)

`l2.ini › [WinDrv.WindowsClient] › FullscreenViewportX` · number · px · *Advanced*

- **Default:** `1024`
- **Allowed:** 640 – 7680 px

### Full screen height (engine) (FullscreenViewportY)

`l2.ini › [WinDrv.WindowsClient] › FullscreenViewportY` · number · px · *Advanced*

- **Default:** `768`
- **Allowed:** 480 – 4320 px
