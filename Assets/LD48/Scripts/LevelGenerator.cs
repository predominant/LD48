using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.LD48.Scripts
{
    public class LevelGenerator : MonoBehaviour
    {
        public int Seed = 1;
        public int MinSections = 5;
        public float Width = 20f;
        public float Height = 10f;
        public float MainOffsetScale = 350f;
        public float OffsetScale = 3f;
        public float HeightStep = 2;
        public float MinBoxScale = 2f;
        public float MaxBoxScale = 4.5f;
        public float BoxRotation = 30f;
        public Transform CameraTransform;
        public GameObject WallPrefab;
        public GameObject PlatformPrefab;
        public LayerMask PlatformLayer;
        public GameObject HealthPackPrefab;
        public LayerMask HealthpackLayer;
        public Vector3 PrefabPoolPosition;
        public Material[] WallMaterials;

        private int _lastHeight = 1;
        
        private Transform PoolTransform;
        private Stack<GameObject> _wallPool = new Stack<GameObject>();

        private WaitForSeconds GenerationWaitTime = new WaitForSeconds(0.0001f);

        private void Awake()
        {
            Random.InitState(this.Seed);
            
            this.transform.position = Vector3.up * this.Height;
            var pool = new GameObject("Pool");
            this.PoolTransform = pool.transform;
            this.PoolTransform.position = this.PrefabPoolPosition;
            
            while (this.CurrentSections < this.MinSections)
                this.GenerateSection(this._lastHeight++, true);
        }

        private void Update()
        {
            if (this.CurrentSections < this.MinSections)
                this.GenerateSection(this._lastHeight++);
        }

        private int CurrentSections => GameObject.FindObjectsOfType<Section>().Length;

        private void GenerateSection(int height = 1, bool initialSpawn = false)
        {
            var sectionObject = new GameObject($"Section {height}");
            var section = sectionObject.AddComponent<Section>();
            section.Height = height;
            section.Generator = this;
            
            // sectionObject.transform.parent = this.transform;

            if (initialSpawn)
            {
                sectionObject.transform.localPosition = Vector3.up * this.Height + Vector3.down * (this.Height * (height - 1));
            }
            else
            {
                // sectionObject.transform.localPosition = Vector3.down * (this.Height * 2);
                var lastSection = GameObject.Find($"Section {height - 1}").transform;
                sectionObject.transform.position = lastSection.position + (Vector3.down * this.Height);
            }

            GameObject wall;
            float mainOffset;
            float offset1, offset2;
            float spawnHeight = 0;

            bool placedPlatform = false;
            bool placedHealthPack = false;

            for (var i = 0f; i < this.Height; i += this.HeightStep)
            {
                spawnHeight = -i;
                
                // mainOffset = Mathf.PerlinNoise((float)i / (float)this.Height / (float)this.HeightStep, (float)height / 10f) - 0.5f;
                mainOffset = Mathf.PerlinNoise((float)height + (float)i / (float)this.Height, 0.5f) - 0.5f;
                mainOffset *= this.MainOffsetScale;
                // Debug.Log($"Main offset: {mainOffset}");
                
                // Left Wall
                offset1 = Mathf.PerlinNoise(0.25f, (float)height / 100f + (float)i / (float)height) - 0.5f;
                offset1 *= this.OffsetScale;
                offset1 += mainOffset;
                wall = this.GetWall();
                wall.transform.parent = sectionObject.transform;
                var leftPos = this.Width / -2f + offset1;
                wall.transform.localPosition = new Vector3(leftPos, spawnHeight, 0);
                wall.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-1f, 1f) * this.BoxRotation);
                wall.transform.localScale = Random.Range(this.MinBoxScale, this.MaxBoxScale) * Vector3.one;
                wall.GetComponentInChildren<Renderer>().material = this.RandomWallMaterial;

                // Right Wall
                offset2 = Mathf.PerlinNoise(0.75f, (float)height / 100f + (float)i / (float)height);
                offset2 *= this.OffsetScale;
                offset2 += mainOffset;
                wall = this.GetWall();
                wall.transform.parent = sectionObject.transform;
                var rightPos = this.Width / 2f + offset2;
                wall.transform.localPosition = new Vector3(rightPos, spawnHeight, 0);
                wall.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-1f, 1f) * this.BoxRotation);
                wall.transform.localScale = Random.Range(this.MinBoxScale, this.MaxBoxScale) * Vector3.one;
                wall.GetComponentInChildren<Renderer>().material = this.RandomWallMaterial;

                if (!placedHealthPack && Mathf.FloorToInt(height) % 2 == 0 && height > 2)
                {
                    placedHealthPack = true;
                    this.SpawnHealthPack(
                        sectionObject.transform,
                        leftPos,
                        rightPos,
                        spawnHeight);
                }
                
                if (!placedPlatform && i > this.Height / 2f && height > 1)
                {
                    placedPlatform = true;
                    this.SpawnPlatform(
                        sectionObject.transform,
                        leftPos,
                        rightPos,
                        spawnHeight);
                }
            }
        }

        private Material RandomWallMaterial => this.WallMaterials[Random.Range(0, this.WallMaterials.Length)];

        public void RemoveSection(int height)
        {
            // var section = this.transform.Find($"Section {height}");
            var section = GameObject.Find($"Section {height}").transform;
            foreach (Transform t in section)
                this.RemoveWall(t.gameObject);
        }

        private GameObject GetWall()
        {
            if (this._wallPool.Count == 0)
                this._wallPool.Push(GameObject.Instantiate(this.WallPrefab));

            var o = this._wallPool.Pop();
            return o;
        }

        private void RemoveWall(GameObject o)
        {
            if (this.PlatformLayer == 1 << o.layer || this.HealthpackLayer == 1 << o.layer)
            {
                GameObject.Destroy(o);
                return;
            }

            o.transform.position = this.PrefabPoolPosition;
            o.transform.parent = this.PoolTransform;
            this._wallPool.Push(o);
        }

        private void SpawnPlatform(Transform parent, float left, float right, float height)
        {
            Debug.Log($"{parent.gameObject.name}: left: {left}, right: {right}");
            var platform = GameObject.Instantiate(this.PlatformPrefab, parent);
            platform.gameObject.name = $"Platform for {parent.gameObject.name}";
            // left = left < 0f ? left + left / 3f : left - left / 3f;
            // right = right < 0f ? right + right / 3f : right - right / 3f;

            var midpoint = (left + right) / 2f;
            var size = Mathf.Abs(left - right) / 3.5f;

            platform.transform.localPosition = new Vector3(
                // Random.Range(this.Width / -2f + left, this.Width / 2f + right),
                Random.Range(midpoint - size, midpoint + size),
                height,
                0f);
        }

        private void SpawnHealthPack(Transform parent, float left, float right, float height)
        {
            var midpoint = (left + right) / 2f;
            var size = Mathf.Abs(left - right) / 3f;

            var pos = new Vector3(
                Random.Range(midpoint - size, midpoint + size),
                height,
                0f);

            Collider[] results = new Collider[1];
            var resultCount = Physics.OverlapSphereNonAlloc(parent.position + pos, 1.2f, results, this.PlatformLayer);
            if (resultCount != 0)
                return;
            
            var pack = GameObject.Instantiate(this.HealthPackPrefab, this.PrefabPoolPosition, Quaternion.identity);
            pack.transform.parent = parent;
            pack.transform.localPosition = pos;
        }
    }
}