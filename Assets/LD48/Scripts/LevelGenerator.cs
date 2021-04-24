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

        private int CurrentSections => this.transform.childCount;

        private void GenerateSection(int height = 1, bool initialSpawn = false)
        {
            var sectionObject = new GameObject($"Section {height}");
            var section = sectionObject.AddComponent<Section>();
            section.Height = height;
            section.Generator = this;
            
            sectionObject.transform.parent = this.transform;
            if (initialSpawn)
                sectionObject.transform.localPosition = Vector3.down * (this.Height * (height - 1));
            else
                sectionObject.transform.localPosition = Vector3.down * (this.Height * 2);

            GameObject wall;
            float mainOffset;
            float offset;
            float spawnHeight = 0;

            for (var i = 0f; i < this.Height; i += this.HeightStep)
            {
                spawnHeight = -i;
                
                // mainOffset = Mathf.PerlinNoise((float)i / (float)this.Height / (float)this.HeightStep, (float)height / 10f) - 0.5f;
                mainOffset = Mathf.PerlinNoise((float)height + (float)i / (float)this.Height, 0.5f) - 0.5f;
                mainOffset *= this.MainOffsetScale;
                // Debug.Log($"Main offset: {mainOffset}");
                
                offset = Mathf.PerlinNoise(0.25f, (float)height / 100f + (float)i / (float)height) - 0.5f;
                offset *= this.OffsetScale;
                offset += mainOffset;
                wall = this.GetWall();
                wall.transform.parent = sectionObject.transform;
                wall.transform.localPosition = new Vector3(this.Width / -2f + offset, spawnHeight, 0);
                wall.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-1f, 1f) * this.BoxRotation);
                wall.transform.localScale = Random.Range(this.MinBoxScale, this.MaxBoxScale) * Vector3.one;
                wall.GetComponentInChildren<Renderer>().material = this.RandomWallMaterial;

                offset = Mathf.PerlinNoise(0.75f, (float)height / 100f + (float)i / (float)height);
                offset *= this.OffsetScale;
                offset += mainOffset;
                wall = this.GetWall();
                wall.transform.parent = sectionObject.transform;
                wall.transform.localPosition = new Vector3(this.Width / 2f + offset, spawnHeight, 0);
                wall.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-1f, 1f) * this.BoxRotation);
                wall.transform.localScale = Random.Range(this.MinBoxScale, this.MaxBoxScale) * Vector3.one;
                wall.GetComponentInChildren<Renderer>().material = this.RandomWallMaterial;
            }
        }

        private Material RandomWallMaterial => this.WallMaterials[Random.Range(0, this.WallMaterials.Length)];

        public void RemoveSection(int height)
        {
            var section = this.transform.Find($"Section {height}");
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
            o.transform.position = this.PrefabPoolPosition;
            o.transform.parent = this.PoolTransform;
            this._wallPool.Push(o);
        }
    }
}