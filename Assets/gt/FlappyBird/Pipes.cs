using System.Collections.Generic;
using UnityEngine;

namespace gt.FlappyBird
{
    public class Pipes : MonoBehaviour
    {
        public GameObject PipePrefab;
        public float StartOffset = 1.0f;
        public float Distance = 2f;
        public int PipesCount = 10;
        public float MinHeight;
        public float MaxHeight;
        public float DestroyAt = -8.0f;
        public Birds Birds;

        [HideInInspector] public float NextPipe;

        private Pipe _lastPipe;

        private readonly List<Pipe> _pipes = new List<Pipe>();

        private void Start()
        {
            for (int i = 0; i < PipesCount; i++)
            {
                CreatePipe(StartOffset + i * Distance);
            }
        }

        private void Update()
        {
            for (int i = 0; i < _pipes.Count; i++)
            {
                var pipe = _pipes[i];

                if (pipe.transform.position.x + 0.3f > Birds.transform.position.x)
                {
                    if (pipe == _lastPipe)
                    {
                        break;
                    }
                    NextPipe = SimpleMath.NormalizePoint(MinHeight, MaxHeight, pipe.transform.position.y);
                    _lastPipe = pipe;
                    break;
                }
            }

            for (int i = 0; i < _pipes.Count; i++)
            {
                var pipe = _pipes[i];

                if (pipe.transform.position.x < DestroyAt)
                {
                    _pipes.Remove(pipe);
                    Destroy(pipe.gameObject);
                    var x = _pipes[_pipes.Count - 1].transform.position.x;
                    CreatePipe(x + Distance);
                }
            }
        }

        private void CreatePipe(float x)
        {
            var position = new Vector3(x, Random.Range(MinHeight, MaxHeight), 0.0f);
            var go = Instantiate(PipePrefab, position, Quaternion.identity, transform);
            _pipes.Add(go.GetComponent<Pipe>());
        }

        public void Restart()
        {
            for (int i = 0; i < _pipes.Count; i++)
            {
                var pipe = _pipes[i];
                Destroy(pipe.gameObject);
            }
            _pipes.Clear();

            Start();
        }
    }
}