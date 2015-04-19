using UnityEngine;

public class AspectFix : MonoBehaviour {
	
	public float aspect = 1.6f;
	
	private Vector3 scale = Vector3.one;
	
	public virtual void Initial() {
		scale = transform.localScale;
	}

	[ContextMenu ("Refresh")]
	public virtual void Refresh() {
		transform.localScale = GetScaleFix();
	}
	
	protected Vector3 GetScaleFix() {
		float width = Screen.height * aspect;
		if(width > Screen.width) {
			float ratio = Screen.width / width;
			return new Vector3(scale.x * ratio, scale.y * ratio, scale.z * ratio);
		}
		return scale;
	}
	
	private void Awake() {
		if (enabled) {
			Initial ();
			Refresh ();
		}
	}
	private void Start () {}
}