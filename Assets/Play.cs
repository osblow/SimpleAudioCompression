using UnityEngine;

public class Play : MonoBehaviour 
{
    private const string c_audioPath = "AsphaltSkid";

    AudioListener m_Listener;             
    AudioSource m_sound;

    private AudioClip m_loadedAudio;

    private System.Action m_playeAudio;


    // Use this for initialization
    void Start () 
	{
        m_Listener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;
        m_sound = m_Listener.gameObject.AddComponent<AudioSource>();

        m_loadedAudio = Resources.Load(c_audioPath) as AudioClip;

    }
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    private void LateUpdate()
    {
        if(null != m_playeAudio)
        {
            m_playeAudio.Invoke();
            m_playeAudio = null;
        }
    }

    public void PlaySound()
    {
        float[] aData = new float[m_loadedAudio.samples];
        m_loadedAudio.GetData(aData, 0);

        CompressionTest.Encode(aData, OnCompressFinish);

        
        
    }

    void OnCompressFinish(byte[] compressedData)
    {
        CompressionTest.Decode(compressedData, OnDecompressFinish);
    }

    void OnDecompressFinish(float[] audioData)
    {
        m_playeAudio = delegate () 
        {
            AudioClip ac = AudioClip.Create("new", audioData.Length, 1, 8000, false);
            ac.SetData(audioData, 0);

            m_sound.clip = ac;
            m_sound.Stop();
            m_sound.Play();
        };
    }
}
