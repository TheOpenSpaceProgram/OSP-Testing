﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
// ReSharper disable All

public class MouseManager : MonoBehaviour {
	[SerializeField] public string load = "Box";
	[SerializeField] public bool test = false;
	[SerializeField] private GameObject Vessel;
	private Camera cam;
	RaycastHit hit;
	// Use this for initialization
	void Start() {
		cam = Camera.main;
	}

	// Update is called once per frame
	void Update() {
		if (test) {
			GameObject[] snappoints = GameObject.FindGameObjectsWithTag("Snappoint");
			foreach (GameObject snappoint in snappoints) {
				Destroy(snappoint.gameObject);
			}
			Vessel.GetComponent<Vessel>().enabled = true;


			Transform[] vesselchilds = Vessel.GetComponentsInChildren<Transform>();
			GameObject lowestChild = Vessel;
			foreach (Transform child in vesselchilds) {
				if (child.position.y < lowestChild.transform.position.y) {
					lowestChild = child.gameObject;
				}
			}
			Mesh mesh = lowestChild.GetComponent<MeshFilter>().mesh;
			Vector3[] vertices = mesh.vertices;
			float lowest = Mathf.Infinity;
			int i = 0;
			while (i < vertices.Length) {
				if(vertices[i].y < lowest) lowest = vertices[i].y;
				i++;
			}
			Instantiate(
				Resources.Load("Plane"),
				new Vector3(0f, lowest - 0.5f, 0f),
				Quaternion.identity
			);

			test = false;
		}
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);


		if (Input.GetMouseButtonDown(0)) {
			if (Physics.Raycast(ray, out hit)) {
				if (hit.collider.CompareTag("Snappoint")) {

					Vector3 spawnpoint = hit.collider.transform.position +
					hit.collider.transform.rotation * Vector3.forward * 0.5f;

					Instantiate(
						Resources.Load("Parts/" + load),
						spawnpoint,
						hit.collider.transform.rotation,
						hit.collider.transform.root
					);
					//If symetry
					if (Mathf.Abs(spawnpoint.x) > 0.1f ||
					    Mathf.Abs(spawnpoint.z) > 0.1f) {
						for (var i = 1; i < 4; i++) {
							Instantiate(
								Resources.Load("Parts/" + load),
								Quaternion.AngleAxis(90f * i, Vector3.up) * spawnpoint,
								Quaternion.AngleAxis(90f * i, Vector3.up) * hit.collider.transform.rotation,
								hit.collider.transform.root
							);
						}
					}
				}
			}
		}
		if (Input.GetMouseButtonDown(1)) {
			if (Physics.Raycast(ray, out hit)) {
				if (hit.collider.transform.parent.CompareTag("Part")) {
					Destroy(hit.collider.transform.parent.gameObject);
				}
			}
		}
	}

	public void ChangeTest() {
		test = true;
	}
}
