using UnityEngine;
//using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(AudioSource))]
public class BulletScriptCS : MonoBehaviour {
	
	#region Fields
	public LayerMask layersImpactDetection, noDecalsLayers;
	public float range = 20,damage = 5, hitPointTimer = 5;
	public AudioClip[] aClips;

	private LayerMask layerForDecals;
	private MeshRenderer sRenderer;
	private AudioSource aSource;
	#endregion

	#region Properties

	#endregion

	#region Unity Events

	void Awake () {
		layerForDecals = RemoveFromMask(layersImpactDetection, noDecalsLayers);

		sRenderer = GetComponent<MeshRenderer>();
		aSource = GetComponent<AudioSource>();
	}

	void Start(){

	}

	void OnEnable(){
		if(sRenderer)
			sRenderer.enabled = false;

		TryHit();
        Invoke("RecycleThis", hitPointTimer);
	}

	void OnDisble(){

	}
	#endregion
	
	#region Logic Functions
	void TryHit () {
		Ray ray = new Ray(transform.position,transform.forward);
		RaycastHit hit;
		
		if(Physics.Raycast(ray,out hit, range,layersImpactDetection)){
			
			Vector3 scale = transform.localScale;
			transform.SetParent(hit.transform.root);
			transform.position = hit.point;
			transform.localScale = scale;
			
			if(aClips.Length > 0){
				int i = Random.Range(0,aClips.Length);
				aSource.clip = aClips[i];
				aSource.Play();
			}

            hit.transform.gameObject.SendMessageUpwards("DoDamage", damage, SendMessageOptions.DontRequireReceiver);
			
			if(ContainLayers(layerForDecals ,hit.collider.gameObject.layer)){
				sRenderer.enabled = true;
				transform.rotation = Quaternion.LookRotation(hit.normal);
			}else{
				//				gameObject.SetActive(false);
				RecycleThis();
			}
		}
	}

	public static LayerMask RemoveFromMask(LayerMask original, LayerMask toBeRomoved)
	{
		LayerMask invertedOriginal = ~original;
		return ~(invertedOriginal | toBeRomoved);
	}
	public static bool ContainLayers(LayerMask original, int toBeChecked){
		return (original & (1 << toBeChecked)) != 0;
	}
	public static bool ContainLayers(LayerMask original, LayerMask toBeChecked){
		return (original & toBeChecked) != 0;
	}
	public void RecycleThis(){
//		Recycle
	}
	#endregion
}
