using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class VFXHandler : NetworkBehaviour
{
    [Header("Misc")]
    public ParticleSystem airTrickPS;

    [Header("Drift Effects")]
    public List<ParticleSystem> particleSystemsBR;
    public List<ParticleSystem> particleSystemsBL;
    public List<ParticleSystem> TireScreechesLtoR;
    public List<ParticleSystem> transitionSparksLtoR;
    public List<Color> turboColors;
    public ParticleSystem driftSparksLeftBack;
    public ParticleSystem driftSparksLeftFront;
    public ParticleSystem driftSparksRightFront;
    public ParticleSystem driftSparksRightBack;
    public List<ParticleSystem> boostFlames;

    // ---------- Public Methods----------

    public void PlayBoostVFX()
    {
        PlayBoostVFXLocal();

        if (ShouldNetwork())
            PlayBoostVFXClientRpc();
    }

    public void StopBoostVFX()
    {
        StopBoostVFXLocal();

        if (ShouldNetwork())
            StopBoostVFXClientRpc();
    }

    public void PlayAirTrickVFX(bool isLeft)
    {
        PlayAirTrickVFXLocal(isLeft);

        if (ShouldNetwork())
            PlayAirTrickVFXClientRpc(isLeft);
    }

    public void ParticleSystemsL()
    {
        ParticleSystemsL_Local();

        if (ShouldNetwork())
            ParticleSystemsL_ClientRpc();
    }

    public void ParticleSystemsR()
    {
        ParticleSystemsR_Local();

        if (ShouldNetwork())
            ParticleSystemsR_ClientRpc();
    }

    public void TireScreechesL()
    {
        TireScreechesL_Local();

        if (ShouldNetwork())
            TireScreechesL_ClientRpc();
    }

    public void TireScreechesR()
    {
        TireScreechesR_Local();

        if (ShouldNetwork())
            TireScreechesR_ClientRpc();
    }

    public void TierTransitionSparksL()
    {
        TierTransitionSparksL_Local();

        if (ShouldNetwork())
            TierTransitionSparksL_ClientRpc();
    }

    public void TierTransitionSparksR()
    {
        TierTransitionSparksR_Local();

        if (ShouldNetwork())
            TierTransitionSparksR_ClientRpc();
    }

    public void ColorDrift(Color c)
    {
        ColorDriftLocal(c);

        if (ShouldNetwork())
            ColorDriftClientRpc(c);
    }

    public void StopAllParticles()
    {
        particleSystemsBR.ForEach(ps => ps.Stop());
        particleSystemsBL.ForEach(ps => ps.Stop());
        TireScreechesLtoR.ForEach(ps => ps.Stop());
        transitionSparksLtoR.ForEach(ps => ps.Stop());
        boostFlames.ForEach(ps => ps.Stop());
        airTrickPS.Stop();
    }

    public void StopDriftVFX()
    {
        StopDriftVFXLocal();

        if (ShouldNetwork())
            StopDriftVFXClientRpc();
    }


    // ---------- Local Methods ----------

    void PlayBoostVFXLocal()
    {
        foreach (var ps in boostFlames)
            if (!ps.isPlaying) ps.Play();
    }

    void StopBoostVFXLocal()
    {
        foreach (var ps in boostFlames)
            if (ps.isPlaying) ps.Stop();
    }

    void PlayAirTrickVFXLocal(bool isLeft)
    {
        var main = airTrickPS.main;
        main.flipRotation = isLeft ? 1 : 0;
        airTrickPS.Play();
    }

    void ParticleSystemsL_Local()
    {
        foreach (var ps in particleSystemsBL)
            if (!ps.isPlaying) ps.Play();
    }

    void ParticleSystemsR_Local()
    {
        foreach (var ps in particleSystemsBR)
            if (!ps.isPlaying) ps.Play();
    }

    void TireScreechesL_Local()
    {
        if(!TireScreechesLtoR[0].isPlaying)
        {
                TireScreechesLtoR[0].Play();
                TireScreechesLtoR[2].Play();

                if (TireScreechesLtoR[1].isPlaying)
                {
                    TireScreechesLtoR[1].Stop();
                    TireScreechesLtoR[3].Stop();
                    particleSystemsBR.ForEach(ps => ps.Stop());
                }
        }
            
        
    }

    void TireScreechesR_Local()
    {
        if(!TireScreechesLtoR[1].isPlaying)
        {
            TireScreechesLtoR[1].Play();
        TireScreechesLtoR[3].Play();

        if (TireScreechesLtoR[0].isPlaying)
        {
            TireScreechesLtoR[0].Stop();
            TireScreechesLtoR[2].Stop();
            particleSystemsBR.ForEach(ps => ps.Stop());
        }
        }        
    }

    void TierTransitionSparksL_Local()
    {
        transitionSparksLtoR[0].Play();
        transitionSparksLtoR[2].Play();
        transitionSparksLtoR[4].Play();
        transitionSparksLtoR[5].Play();
    }

    void TierTransitionSparksR_Local()
    {
        transitionSparksLtoR[1].Play();
        transitionSparksLtoR[3].Play();
        transitionSparksLtoR[5].Play();
        transitionSparksLtoR[6].Play();
    }

    void ColorDriftLocal(Color c)
    {
        void SetColor(ParticleSystem ps)
        {
            var main = ps.main;
            main.startColor = c;
        }

        particleSystemsBL.ForEach(SetColor);
        particleSystemsBR.ForEach(SetColor);
        TireScreechesLtoR.ForEach(SetColor);
        transitionSparksLtoR.ForEach(SetColor);
    }

    void StopDriftVFXLocal()
    {
        particleSystemsBL.ForEach(ps => ps.Stop());
        particleSystemsBR.ForEach(ps => ps.Stop());
        TireScreechesLtoR.ForEach(ps => ps.Stop());
        transitionSparksLtoR.ForEach(ps => ps.Stop());
    }


    // ---------- ClientRPCs ----------

    [ClientRpc] void PlayBoostVFXClientRpc() => PlayBoostVFXLocal();
    [ClientRpc] void StopBoostVFXClientRpc() => StopBoostVFXLocal();
    [ClientRpc] void PlayAirTrickVFXClientRpc(bool isLeft) => PlayAirTrickVFXLocal(isLeft);
    [ClientRpc] void ParticleSystemsL_ClientRpc() => ParticleSystemsL_Local();
    [ClientRpc] void ParticleSystemsR_ClientRpc() => ParticleSystemsR_Local();
    [ClientRpc] void TireScreechesL_ClientRpc() => TireScreechesL_Local();
    [ClientRpc] void TireScreechesR_ClientRpc() => TireScreechesR_Local();
    [ClientRpc] void TierTransitionSparksL_ClientRpc() => TierTransitionSparksL_Local();
    [ClientRpc] void TierTransitionSparksR_ClientRpc() => TierTransitionSparksR_Local();
    [ClientRpc] void ColorDriftClientRpc(Color c) => ColorDriftLocal(c);
    [ClientRpc] void StopDriftVFXClientRpc() => StopDriftVFXLocal();



    // ---------- Utility ----------

    bool ShouldNetwork()
    {
        return NetworkManager.Singleton != null &&
               NetworkManager.Singleton.IsListening &&
               IsOwner;
    }
}
