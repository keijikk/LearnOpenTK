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
            // Position         Texture coordinates
            0.5f, 0.5f, 0.0f, 1.0f, 1.0f, // top right
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f, 0.5f, 0.0f, 0.0f, 1.0f // top left 
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _elementBufferObject;
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;
        private Texture _texture;

        #endregion

        #region -- Constructors --

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
        }

        #endregion

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

            // EBOを作成し、バインドしてデータをアップロード
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
                BufferUsageHint.StaticDraw);

            // shaderのコンパイル
            _shader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
            _shader.Use();

            _texture = new Texture();
            _texture.Use();

            // VAOにVBOにまとめる
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            // GL.EnableVertexAttribArrayを使用して頂点属性を有効にし、引数として頂点属性の位置を指定
            //GL.EnableVertexAttribArray(0);
            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);

            // vertexをどのように解釈するかを指定する
            // index: 頂点属性をのindex　shader.vertで指定したlayout (location = 0)を指定していたので、0を設定
            // size: shader.vertのvertex attributeがvec3なので、3を指定
            // VertexAttribPointerType: データタイプはfloat
            // normalized: floatにキャストするときに整数データ値を正規化する場合にtrue. データがfloatの場合は無効
            //    gl.BYTEおよびgl.SHORT型の場合、trueの場合、値を[-1、1]に正規化します。
            //    gl.UNSIGNED_BYTEおよびgl.UNSIGNED_SHORT型の場合、trueの場合、値を[0、1]に正規化します。
            //    gl.FLOATおよびgl.HALF_FLOAT型のgl.FLOAT 、このパラメータは無効です。
            // stride: 頂点間のデータ幅: 5点存在する
            // offset: バッファ内の位置データの開始位置
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);


            // Next, we also setup texture coordinates. It works in much the same way.
            // We add an offset of 3, since the first vertex coordinate comes after the first vertex
            // and change the amount of data to 2 because there's only 2 floats for vertex coordinates
            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));


            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // OnLoadのカラーセットでクリア。必ず最初に呼ぶ。
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(_vertexArrayObject);

            _texture.Use();
            _shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
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
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);
            base.OnUnload(e);
        }
    }
}