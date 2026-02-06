# Metamorphopsia VR - Lesson Agenda

---

## Lesson 1: Unity Environment & VR Setup

| Topic | Question |
|-------|----------|
| Unity Editor layout (Hierarchy, Inspector, Scene view) | Q1 |
| GameObjects and Components | Q1 |
| Raycast basics - detecting objects | Q1 |
| UI Buttons and click events | Q1 |
| VR setup differences (Quest vs Vive) | Q2 |
| Eye blocking (left/right selection) | Q9 |

---

## Lesson 2: Mesh Fundamentals & Amsler Grid

| Topic | Question |
|-------|----------|
| What is a Mesh (vertices, triangles, UVs) | Q8 |
| Vertex index formula: `index = x + y * width` | Q3 |
| Creating Amsler Grid 20×20 programmatically | Q3 |
| Grid shader for drawing lines | Q3 |
| Subdivision (11×11 → 21×21 → 41×41) | Q7 |
| Save/Load mesh to JSON | Q8 |

---


## Lesson 3: Vertex Manipulation & Interaction

| Topic | Question |
|-------|----------|
| Visual markers at each vertex | Q4 |
| Selecting vertices with raycast | Q4 |
| Moving vertices with arrow keys | Q4 |
| Influence radius (neighbors move proportionally) | Q4 |
| Boundary constraints (edges locked) | Q17 |
| Audio feedback on selection/movement | Q4 |

---

## Lesson 4A: GPU Basics - Compute Shaders

| Topic | Question |
|-------|----------|
| CPU vs GPU - when to use which | Tech Q6 |
| First compute shader (`#pragma kernel`) | Tech Q6 |
| ComputeBuffer - send data to GPU | Tech Q18 |
| Thread ID (`SV_DispatchThreadID`) | Tech Q18 |
| Dispatch groups calculation | Tech Q18 |
| `numthreads` and thread math | Tech Q18 |

---

## Lesson 4B: Rasterization & UV Remapping

| Topic | Question |
|-------|----------|
| What is rasterization (shape → pixels) | Tech Q1 |
| Barycentric coordinates | Tech Q2 |
| Interpolation (bilinear vs barycentric vs quadrilateral) | Tech Q2 |
| Why quadrilateral is smoother (no diagonal seam) | Tech Q2 |
| UV remapping concept (counter-distortion) | Tech Q5, Q10, Q13 |
| Perspective correction | Tech Q4 |
| RenderTexture output | Tech Q7, Q14, Q16 |
| Mesh vs pixel distortion | Tech Q8 |

---

## Lesson 5: Eye Tracking, Data & Video

| Topic | Question |
|-------|----------|
| Eye tracking basics (gaze ray) | Q5 |
| SRanipal SDK for Vive Pro Eye | Q5 |
| Grid follows gaze direction | Q5 |
| Recording gaze data | Q12 |
| Export to JSON and CSV | Q12 |
| Data analysis (RMS, fixation, saccade) | Q12 |
| Apply distortion to images | Q10 |
| Apply distortion to live video | Q11 |
| Video quality, latency, frame rate | Q13 |

---

# Questions Map

## Part 1: الدروس

| # | Topic | Lesson |
|---|-------|--------|
| 1 | Unity environment, buttons, raycast | L1 |
| 2 | Connecting Unity with Vive Pro Eye | L1 |
| 3 | Creating Amsler Grid 20×20 | L2 |
| 4 | Vertex control, neighbor movement, sound | L3 |
| 5 | Eye tracking, grid follows gaze | L5 |
| 7 | Grid subdivision | L2 |
| 8 | Saving the grid (mesh to JSON) | L2 |
| 9 | Eye blocking (left/right selection) | L1 |
| 10 | Apply distortions to image | L4B, L5 |
| 11 | Apply distortions to video | L5 |
| 12 | Excel calculations, JSON export, RMS | L5 |
| 13 | Video quality, latency, frame rate | L5 |

## Part 2: الاسئلة

| # | Concept | Lesson |
|---|---------|--------|
| 1 | Rasterization | L4B |
| 2 | Quadrilateral vs Barycentric interpolation | L4B |
| 3 | MVC pattern | L1 |
| 4 | Perspective correction | L4B |
| 5 | UV mapping vs Displacement mapping | L4B |
| 6 | Compute shader role | L4A |
| 7 | Render Texture | L4B |
| 8 | Mesh vs Pixel distortion | L4B |
| 9 | Compute shader warp | L4A |
| 10 | Counter distortion | L4B |
| 11 | Distorted mesh = Amsler grid | L2, L3 |
| 12 | Stereo rendering | L1 |
| 13 | UV remapping basics | L4B |
| 14 | Writing UVTexture on GPU | L4A, L4B |
| 15 | UV change on vertex move | L3, L4B |
| 16 | Mesh to texture conversion | L4B |
| 17 | Boundary constraints | L3 |
| 18 | Threads, Dispatch, Buffer, Thread ID | L4A |
