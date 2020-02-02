﻿#if !__MonoCS__
// class of a special surface chart, (uniform grid in x-y direction)
// version 0.1


using System.Collections;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Computator.NET.Charting.Chart3D.Chart3D
{
    internal class UniformSurfaceChart3D : SurfaceChart3D
    {
        private int m_nGridXNo, m_nGridYNo; // the grid number on each axis

        public void SetPoint(int i, int j, float x, float y, float z)
        {
            var nI = j*m_nGridXNo + i;
            m_vertices[nI].x = x;
            m_vertices[nI].y = y;
            m_vertices[nI].z = z;
        }

        public void SetZ(int i, int j, float z)
        {
            m_vertices[j*m_nGridXNo + i].z = z;
        }

        public void SetColor(int i, int j, Color color)
        {
            var nI = j*m_nGridXNo + i;
            m_vertices[nI].color = color;
        }

        public void SetGrid(int xNo, int yNo, float xMin, float xMax, float yMin, float yMax)
        {
            SetDataNo(xNo*yNo);
            m_nGridXNo = xNo;
            m_nGridYNo = yNo;
            m_xMin = xMin;
            m_xMax = xMax;
            m_yMin = yMin;
            m_yMax = yMax;
            var dx = (m_xMax - m_xMin)/((float) xNo - 1);
            var dy = (m_yMax - m_yMin)/((float) yNo - 1);
            for (var i = 0; i < xNo; i++)
            {
                for (var j = 0; j < yNo; j++)
                {
                    var xV = m_xMin + dx*i;
                    var yV = m_yMin + dy*j;
                    m_vertices[j*xNo + i] = new Vertex3D();
                    SetPoint(i, j, xV, yV, 0);
                }
            }
        }

        // convert the uniform surface chart to a array of Mesh3D (only one element)
        public ArrayList GetMeshes()
        {
            var meshes = new ArrayList();
            var surfaceMesh = new ColorMesh3D();

            surfaceMesh.SetSize(m_nGridXNo*m_nGridYNo, 2*(m_nGridXNo - 1)*(m_nGridYNo - 1));

            for (var i = 0; i < m_nGridXNo; i++)
            {
                for (var j = 0; j < m_nGridYNo; j++)
                {
                    var nI = j*m_nGridXNo + i;
                    var vert = m_vertices[nI];
                    m_vertices[nI].nMinI = nI;
                    surfaceMesh.SetPoint(nI, new Point3D(vert.x, vert.y, vert.z));
                    surfaceMesh.SetColor(nI, vert.color);
                }
            }
            // set triangle
            var nT = 0;
            for (var i = 0; i < m_nGridXNo - 1; i++)
            {
                for (var j = 0; j < m_nGridYNo - 1; j++)
                {
                    var n00 = j*m_nGridXNo + i;
                    var n10 = j*m_nGridXNo + i + 1;
                    var n01 = (j + 1)*m_nGridXNo + i;
                    var n11 = (j + 1)*m_nGridXNo + i + 1;

                    surfaceMesh.SetTriangle(nT, n00, n10, n01);
                    nT++;
                    surfaceMesh.SetTriangle(nT, n01, n10, n11);
                    nT++;
                }
            }
            meshes.Add(surfaceMesh);
            AddAxesMeshes(meshes);
            return meshes;
        }
    }
}
#endif