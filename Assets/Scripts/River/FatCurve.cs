using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class FatCurve {
    internal struct Tri {
        public Vector3 va, vb, vc;
        public Vector3 uIndices;
        public Vector3 vIndices;

        public Tri(Vector3 va, Vector3 vb, Vector3 vc, 
            Vector3 uIndices,
            Vector3 vIndices)
        {
            this.va = va;
            this.vb = vb;
            this.vc = vc;
            this.uIndices = uIndices;
            this.vIndices = vIndices;
        }

        public bool ContainsPoint(Vector3 p)
        {
            var abp = (vb.x - va.x) * (p.z - va.z) - (vb.z - va.z) * (p.x - va.x);
            abp = Mathf.Sign(abp);
            var bcp = (vc.x - vb.x) * (p.z - vb.z) - (vc.z - vb.z) * (p.x - vb.x);
            bcp = Mathf.Sign(bcp);
            var cap = (va.x - vc.x) * (p.z - vc.z) - (va.z - vc.z) * (p.x - vc.x);
            cap = Mathf.Sign(cap);
            return abp == bcp && bcp == cap;
        }

        float Cross2D(Vector3 a, Vector3 b)
        {
            return a.x * b.z - a.z * b.x;
        }

        Vector3 Barycentric(Vector3 p)
        {
            var t1 = Cross2D(p - vc, vb - vc) / Cross2D(va - vc, vb - vc);
            var t2 = Cross2D(p - vc, vc - va) / Cross2D(va - vc, vb - vc);
            var t3 = 1 - t1 - t2;
            return new Vector3(t1, t2, t3);
        }

        public float getUCoordinate(Vector3 p)
        {
            var coordinates = Barycentric(p);
            return Vector3.Dot(coordinates, uIndices);
        }

        public float getVCoordinate(Vector3 p)
        {
            var coordinates = Barycentric(p);
            return Vector3.Dot(coordinates, vIndices);
        }

        public (float, float) getUVCoordinates(Vector3 p)
        {
            var coordinates = Barycentric(p);
            return (Vector3.Dot(coordinates, uIndices),
                Vector3.Dot(coordinates, vIndices));
        }
    }

    public List<Tri> triangles;
    public AnimationCurve profile;

    public FatCurve(List<Tri> triangles, AnimationCurve profile)
    {
        this.triangles = triangles;
        this.profile = profile;
    }

    public Mesh toMesh(ChunkManager chunks)
    {
        var vertices = triangles.SelectMany(triangle => new List<Vector3> { triangle.va, triangle.vb, triangle.vc })
                .Select(vertex => {
                    vertex.y = chunks.GetHeight(vertex);
                    return vertex;
                }).ToArray();
        return generateMesh(vertices);
    }

    public Mesh toFlatMesh() {
        var vertices = triangles.SelectMany(triangle => new List<Vector3> { triangle.va, triangle.vb, triangle.vc })
            .ToArray();
        return generateMesh(vertices);
    }

    private Mesh generateMesh(Vector3[] vertices) {
        var uvs = triangles.SelectMany(triangle =>
        {
            var us = triangle.uIndices;
            var vs = triangle.vIndices;
            return new List<Vector2>
            {
                new Vector2(us.x, vs.x),
                new Vector2(us.y, vs.y),
                new Vector2(us.z, vs.z)
            };
        }).ToArray();
        var colors = triangles.SelectMany(triangle => new List<Color32> {
                triangle.uIndices.x == 0 ? Color.red : Color.blue,
                triangle.uIndices.y == 0 ? Color.red : Color.blue,
                triangle.uIndices.z == 0 ? Color.red : Color.blue,
            }).ToArray();
        var tris = Enumerable.Range(0, vertices.Count()).ToArray();
        var normals = Enumerable.Repeat(Vector3.up, vertices.Count()).ToArray();
        var mesh = new Mesh
        {
            vertices = vertices,
            triangles = tris,
            normals = normals,
            name = "Fat curve",
            colors32 = colors,
            uv = uvs,
        };
        return mesh;
    }
}