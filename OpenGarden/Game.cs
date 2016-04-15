using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Audio;

namespace OpenGarden
{
    class Game : GameWindow
    {
        //Random seed
        Random rnd = new Random(System.Environment.TickCount);

        //Audio engine.
        public GameAudio GA;

        //Level
        private Level level;

        //GPU addresses to our program and shaders
        private int pgmID;  //program ID
        private int vsID;   //vertex shader
        private int fsID;   //fragment shader

        //These will store the addresses for the vs shader compiled into our gpu program.
        int attribute_vcol; //use this to push the vtx col info to the gpu pgm. (compiled vs shader)
        int attribute_vpos; //use this to push the vtx pos info to the gpu pgm. (compiled vs shader)
        int uniform_modelview;   //modelview matrix for the vs! :)
        //To get the addresses for each variable, we use the GL.GetAttribLocation and 
        //GL.GetUniformLocation functions. Each takes the program's ID and the name 
        //of the variable in the shader.

        //Geometry is sent over via Vertex Buffer Object (VBO)'s
        //They are requested from the gpu and then we bind to them
        //similar to the shader attributes
        int vbo_position;   //position buffer
        int vbo_color;      //color buffer
        int vbo_mview;      //modelview buffer
        int ibo_elements;   //vert index buffer
        
        //The CPU/game-side vars to store the actual data for the buffers
        Vector3[] vbo_position_data;        //vert position array
        Vector3[] vbo_color_data;           //vert color array
        int[]     ibo_data;                 //the vert index array

        //Windowstate Flags
        bool fullscreen;    //Toggled & checked by ToggleFullScreen method.

        //Input related vars and flags 
        OpenTK.Input.KeyboardState kb_state_old;  //important to track for keydown&keyup states 

        //Time (updated in OnUpdateFrame via the passed event
        double elapsedTime = 0f;

        public Game() : base(512, 512, new GraphicsMode(32, 24, 0, 4))
        {
            return;
        }

        //Do the game logic/simulation
        protected void Tick(double deltaTime)
        {
            //Do level tick
            level.Tick();
            foreach (Volume v in level.scene)
            {
                v.CalculateModelMatrix();
                v.ViewProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1f, ClientSize.Width / (float)ClientSize.Height, 1.0f, 40.0f);
                v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewProjectionMatrix;
            }

            //Create dynamic list objects for vert attributes we can use
            //to aggregate/build the data for the buffers
            List<Vector3> vpos_list = new List<Vector3>();
            List<Vector3> vcol_list = new List<Vector3>();
            List<int> vindex_list = new List<int>();

            int vertCount = 0;

            //Aggregate the vert attr data into the lists (sum the vert count)
            foreach (Volume v in level.scene)
            {
                vpos_list.AddRange(v.GetVPosition_Data());
                vcol_list.AddRange(v.GetVColor_Data());
                vindex_list.AddRange(v.GetVIndex_Data(vertCount));
                vertCount += v.VertCount;
            }

            //Convert the list data to arrays for the buffers
            vbo_position_data = vpos_list.ToArray();
            vbo_color_data = vcol_list.ToArray();
            ibo_data = vindex_list.ToArray();
        }

        //Updates the window attributes and projection matrix
        //Set doingResize flag to true for the duration
        protected void ToggleFullscreen()
        {
            fullscreen = fullscreen ? false : true; //toggle fullscreen state
            WindowState = fullscreen ? WindowState.Maximized : WindowState.Normal;
            WindowBorder = fullscreen ? WindowBorder.Hidden : WindowBorder.Resizable;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4,
                        ClientRectangle.Width / (float)ClientRectangle.Height,
                        1.0f,
                        64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref projection);
        }

        //Runs once when first loading the game.
        //1.    Setup the GPU prog. & resources.
        //2.    Initialize the camera/modelview matrix.
        //3.    Initialize the CPU-side VBO data.
        //4.    Set Window styles/attributes.
        //5.    Set GL features/attributes.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            //1.    Setup the GPU prog. & resources.
            InitProgram();

            //1a.   Initialize game audio
            GA = new GameAudio();

            //1a    Load level into level.scene;
            //level = new Level_LocalSpin();
            level = new Level_Plant();
            level.LoadLevel(GA);
            
            //4.    Set Window styles/attributes.
            ToggleFullscreen();

            //5.    Set GL features/attributes.
            GL.ClearColor(Color4.SlateGray);    //the color to use for clearing the buffer (depth too?)
            GL.PointSize(5);                    //If drawing using points, the point radius
            GL.Enable(EnableCap.DepthTest);     //!to-learn!
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        //Do the game logic in here
        //1.    Process any input.
        //  1a.     Do game logic
        //2.    Bind-to, write, then point to shader_attr. each VBO.
        //3.    Send modelview via UniformMatrix4... !to-learn! this seems blackbox
        //4.    Bind-to null to release any resource handles.
        //5.    Set the GPU to run the program.
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            elapsedTime += e.Time;
            base.OnUpdateFrame(e);

            //Get the keyboard state
            var kb_state_cur = OpenTK.Input.Keyboard.GetState();
            
            //1.    Process only new input events if any (keypress/release).
            if (kb_state_cur[Key.Escape] && !kb_state_old[Key.Escape])
            {
                Exit();
            }
            if ((kb_state_cur[Key.LAlt] && kb_state_cur[Key.Enter]) 
                && !(kb_state_old[Key.LAlt] && kb_state_old[Key.Enter]))
            {
                Console.WriteLine("Recieved Input: leftalt + enter down");
                ToggleFullscreen();
            }
            kb_state_old = kb_state_cur;

            //1a    Do game logic/simulation
            Tick(e.Time);

            //2.    Bind, write, specify data format for each arraybuffer VBO.
            //  BindBuffer call returns a resource from the gpu we can write to.
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            //  BufferData copies the data to the gpu 
            GL.BufferData<Vector3>(
                BufferTarget.ArrayBuffer,                                   //      buffertype      : !to-learn! array (what are the other types!?)
                (IntPtr)(vbo_position_data.Length * Vector3.SizeInBytes),   //      size (in bytes) : array length * element size
                vbo_position_data,                                          //      src data array  : CPU-side array from which to grab the data
                BufferUsageHint.StaticDraw);                                //      usage hint)     : !to-learn!
            //  Point vbo address to VS shader attr. address and specify data layout (3 float types)
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
            //  Same process for the color vbo & shader attribute
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                (IntPtr)(vbo_color_data.Length * Vector3.SizeInBytes),
                vbo_color_data,
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 3, VertexAttribPointerType.Float, true, 0, 0);

            //  The vertex index buffer is used by the GL.DrawElements rather than
            //  being an attribute of a vertex so it doesn't require a call to
            //  glVertexAttribPointer. 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer,
                (IntPtr)(ibo_data.Length * sizeof(int)),
                ibo_data,
                BufferUsageHint.StaticDraw);

            //4.    Bind-to null to release any resource handles.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            //5.    Set the GPU to run the program.
            GL.UseProgram(pgmID);
        }

        //  Do the rendering
        //1.    Clear the color/depth buffers.
        //2.    Enable/Toggle ON the relevant VBO's for this draw-call.
        //3.    Call DrawArrays to start drawing from VBO's using primitive type.
        //4.    Disable/Toggle OFF the VBO's used for the draw-call.
        //5.    Swap buffers
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            //1.    Clear buffer.  Whats the depth buffer mask look like?
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //2.    Enable/Toggle ON the relevant VBO resources for this draw-call.
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.EnableVertexAttribArray(attribute_vcol);

            //3.    Call DrawArrays to start drawing from VBO's using primitive type.
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            //GL.DrawElements(PrimitiveType.Triangles, ibo_data.Length, DrawElementsType.UnsignedInt, 0);
            int v_index = 0;

            foreach (Volume v in level.scene)
            {
                GL.UniformMatrix4(uniform_modelview, false, ref v.ModelViewProjectionMatrix);
                GL.DrawElements(v.PrimType, v.IndiceCount, DrawElementsType.UnsignedInt, v_index * sizeof(uint));
                v_index += v.IndiceCount;
            }
            
            //4.    Disable/Toggle OFF the VBO's used for the draw-call.
            GL.DisableVertexAttribArray(attribute_vpos);
            GL.DisableVertexAttribArray(attribute_vcol);

            GL.Flush(); //!to-learn! what is this shit!?

            //5.    Swap buffers
            SwapBuffers();
        }

        //Create all the gpu resources (via commands to GL)
        //Reference to the gpu resources are stored via ints representing their address on the gpu
        //The steps are:
        //  1.  Request a new program from the GPU.
        //  2.  Load/Compile/Attach the shaders (see LoadShader for sub-steps)
        //  3.  Link the program (links all the compiled&attached code like C)
        //  4.  Request/store addresses to shader parameters/attributes defined in glsl
        //  5.  Generate & store VertexBufferObject (VBO) addresses.
        void InitProgram()
        {
            //  1.  Request a new program from the GPU.
            pgmID = GL.CreateProgram();

            //  2.  Load/Compile/Attach the shaders (see LoadShader for sub-steps)
            LoadShader("vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
            LoadShader("fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);

            //  3.  Link the program (links all the compiled&attached code like C)
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID)); //report any problems linking

            //  4.  Request/store addresses to shader parameters/attributes defined in glsl            
            attribute_vcol = GL.GetAttribLocation(pgmID, "vColor");
            attribute_vpos = GL.GetAttribLocation(pgmID, "vPosition");
            uniform_modelview = GL.GetUniformLocation(pgmID, "modelview");
            //      Catch & output any problems getting the addresses
            if (attribute_vcol == -1 || attribute_vpos == -1 || uniform_modelview == -1)
            {
                Console.WriteLine("Error binding attributes.");
            }

            //  5.  Generate & store VertexBufferObject (VBO) addresses.
            //      No buffer objects are associated with the returned buffer object names/addresses
            //      until they are first bound by calling glBindBuffer.
            GL.GenBuffers(1, out vbo_color);
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out vbo_mview);
            GL.GenBuffers(1, out ibo_elements);
        }

        //Load the shader
        //Shader loading pattern is (via calls to GL): 
        //  1 create shader & return address 
        //  2 load shader code from glsl files,
        //  3 compile, 
        //  4 attach to program (different than link which happens later)
        void LoadShader(String filename, ShaderType type, int program, out int address)
        {
            //Ask GL to allocate? the shader type
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                //point the allocated shader to the code
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);                       //compile it!
            GL.AttachShader(program, address);               //add it to our gpu program
            Console.WriteLine(GL.GetShaderInfoLog(address)); //print info on whether the shader compiled
        }
    }
}
