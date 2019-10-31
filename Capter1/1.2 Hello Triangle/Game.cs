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
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, 0.0f, //Bottom-right vertex
            0.0f, 0.5f, 0.0f //Top vertex
        };

        private int _vertexBufferObject;
        private int VertexArrayObject;

        private Shader _shader;

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

            // shaderのコンパイル
            _shader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");

            // 1. bind vertex array
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            // 2. copy our vertices array in a buffer for OpenGL to use
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.StaticDraw);

            // 3. then set our vertex attributes pointers

            // vertexをどのように解釈するかを指定する
            // index: 頂点属性をのindex　shader.vertで指定したlayout (location = 0)を指定していたので、0を設定
            // size: shader.vertのvertex attributeがvec3なので、3を指定
            // VertexAttribPointerType: データタイプはfloat
            // normalized: floatにキャストするときに整数データ値を正規化する場合にtrue. データがfloatの場合は無効
            //    gl.BYTEおよびgl.SHORT型の場合、trueの場合、値を[-1、1]に正規化します。
            //    gl.UNSIGNED_BYTEおよびgl.UNSIGNED_SHORT型の場合、trueの場合、値を[0、1]に正規化します。
            //    gl.FLOATおよびgl.HALF_FLOAT型のgl.FLOAT 、このパラメータは無効です。
            // stride: 頂点間のデータ幅
            // offset: バッファ内の位置データの開始位置
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            // GL.EnableVertexAttribArrayを使用して頂点属性を有効にし、引数として頂点属性の位置を指定
            // 頂点属性はデフォルトで無効になっています。
            GL.EnableVertexAttribArray(0);

            _shader.Use();

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // OnLoadのカラーセットでクリア。必ず最初に呼ぶ。
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Bind the shader
            _shader.Use();

            // Bind the VAO
            GL.BindVertexArray(VertexArrayObject);

            // And then call our drawing function.
            //   How many vertices you want to draw. 3 for a triangle.
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            Context.SwapBuffers();
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
            GL.DeleteBuffer(_vertexBufferObject);
            base.OnUnload(e);
        }
    }
}