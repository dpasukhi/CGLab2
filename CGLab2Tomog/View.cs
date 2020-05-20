using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace CGLab2Tomog
{
    class View
    {
        private int VBOtexture;
        private Bitmap textureImage;
        public static void SetupView(int width, int height)
        {
            GL.ClearColor(Color.White);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.x, 0, Bin.y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }
        Color TransferFunction(short value)
        {
            int min = 0;
            int max = 2000;
            if (min == max)
                max++;
            int newVal = Clamp((value - min) * 255 / (max - min), 0, 255);
            return Color.FromArgb(255, newVal, newVal, newVal);
        }

        public void DrawQuads(int layerNumber)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(PrimitiveType.Quads);
            for (int x_coord = 0; x_coord < Bin.x - 1; x_coord++)
                for (int y_coord = 0; y_coord < Bin.y - 1; y_coord++)
                {
                    short value;

                    //Вершина 1
                    value = Bin.array[x_coord + y_coord * Bin.x + layerNumber * Bin.x * Bin.y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord);

                    //Веришна 2
                    value = Bin.array[x_coord + (y_coord + 1) * Bin.x + layerNumber * Bin.x * Bin.y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord + 1);

                    //Вершина 3
                    value = Bin.array[(x_coord + 1) + (y_coord + 1) * Bin.x + layerNumber * Bin.x * Bin.y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord + 1, y_coord + 1);

                    //Вершина 4
                    value = Bin.array[(x_coord + 1) + y_coord * Bin.x + layerNumber * Bin.x * Bin.y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord + 1, y_coord);
                }
            GL.End();
        }
        public void DrawStripQuads(int layerNumbers)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(PrimitiveType.QuadStrip);
            short value;
            for (int x_coord = 0; x_coord < Bin.x - 1; ++x_coord)
            {
                value = Bin.array[x_coord + layerNumbers * Bin.x * Bin.y];
                GL.Color3(TransferFunction(value));
                GL.Vertex2(x_coord, 0);
                for (int y_coord = 0; y_coord < Bin.y -1; ++y_coord)
                {
                    //1 вершина
                    value = Bin.array[x_coord + (y_coord + 1) * Bin.x + layerNumbers * Bin.x * Bin.y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord +1);
                    //2 вершина
                    value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.x + layerNumbers * Bin.x * Bin.y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord + 1, y_coord + 1);
                }
            }
            value = Bin.array[Bin.x - 1 + layerNumbers * Bin.x * Bin.y];
            GL.Color3(TransferFunction(value));
            GL.Vertex2(Bin.x - 1, 0);

            GL.End();
        }
        public int Clamp(int value, int min, int max)
        {
            return (value<min)?min:(value > max)?max:value;
        }
        public void Load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
            BitmapData data = textureImage.LockBits(
                new System.Drawing.Rectangle(0, 0, textureImage.Width, textureImage.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);
            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            ErrorCode Er = GL.GetError();
            string str = Er.ToString();
        }
        public void generateTextureImage(int layerNumber)
        {
            textureImage = new Bitmap(Bin.x, Bin.y);
            for (int i = 0; i < Bin.x; ++i)
            {
                for (int j = 0; j < Bin.y; ++j)
                {
                    int pixelNumber = i + j * Bin.x + layerNumber * Bin.x * Bin.y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber]));
                }
            }
        }
        public void DrawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.y);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.x, Bin.y);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.x, 0);
            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
    }

}
