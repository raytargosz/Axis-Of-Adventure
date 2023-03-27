using UnityEngine;

[CreateAssetMenu(menuName = "Surface Audio Manager")]
public class SurfaceAudioManager : ScriptableObject
{
    [System.Serializable]
    public class SurfaceData
    {
        public string surfaceTag;
        public AudioClip[] walkSounds;
        public AudioClip jumpSound;
        public AudioClip landSound;
    }

    [SerializeField] private SurfaceData[] surfaces;


    public AudioClip GetWalkSound(string surfaceTag)
    {
        SurfaceData surface = GetSurface(surfaceTag);
        return surface == null ? null : surface.walkSounds[Random.Range(0, surface.walkSounds.Length)];
    }

    public AudioClip GetJumpSound(string surfaceTag)
    {
        SurfaceData surface = GetSurface(surfaceTag);
        return surface == null ? null : surface.jumpSound;
    }

    public AudioClip GetLandSound(string surfaceTag)
    {
        SurfaceData surface = GetSurface(surfaceTag);
        return surface == null ? null : surface.landSound;
    }

    private SurfaceData GetSurface(string surfaceTag)
    {
        foreach (SurfaceData surface in surfaces)
        {
            if (surface.surfaceTag == surfaceTag)
            {
                return surface;
            }
        }
        return null;
    }
}