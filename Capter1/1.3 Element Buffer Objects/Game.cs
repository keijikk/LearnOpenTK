using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace LearnOpenTK
{
    class Game : GameWindow
    {
        #region -- Private Fields --

        private readonly float[] _vertices =
        {
            0.5f, 0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f, 0.5f, 0.0f // top left
        };

        uint[] _indices =
        {
            // note that we start from 0!
            0, 1, 3, // first triangle
            1, 2, 3 // second triangle
        };

        private int _vertexBufferObject;
        private int _vertexArrayObject;

        private Shader _shader;

        // Add a handle for the EBO
        private int _elementBufferObject;

        #endregion

        #region -- Constructors --

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
        }

        #endregion

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }


        /// <summary>
        /// 最初に1回呼ばれる
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // クリアされた後の色を指定
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // VBOを作成し、VBOに頂点データをバインドして、データをバッファにアップロードする
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.StaticDraw);

            // EBOを作成してバインドをしてバッファにアップロード
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(float), _indices,
                BufferUsageHint.StaticDraw);


            // shaderのコンパイル
            _shader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
            _shader.Use();

            // VAOにVBOとVEOをまとめる
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            // 再度VBO、EBOをバインドする
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);


            // draw rectangle
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // OnLoadのカラーセットでクリア。必ず最初に呼ぶ。
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Bind the shader
            _shader.Use();

            // Bind the VAO
            GL.BindVertexArray(_vertexArrayObject);

            // Then replace your call to DrawTriangles with one to DrawElements
            // Arguments:
            //   Primitive type to draw. Triangles in this case.
            //   How many indices should be drawn. Six in this case.
            //   Data type of the indices. The indices are an unsigned int, so we want that here too.
            //   Offset in the EBO. Set this to 0 because we want to draw the whole thing.
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            // GL.Viewport maps the NDC to the window.
            // NDC:Normal Device Coordinate -1～1の間の座標系からwindowの座標にマップする
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            _shader.Dispose();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteBuffer(_vertexArrayObject);
            GL.DeleteVertexArray(_shader.Handle);

            base.OnUnload(e);
        }
    }
}