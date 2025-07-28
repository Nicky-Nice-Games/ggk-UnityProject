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

    public List<ParticleSystem> boostFlames;

    // ---------- Public Methods ----------

    public void PlayBoostVFX()
    {
        PlayBoostVFXLocal();
        if (ShouldNetwork()) PlayBoostVFXClientRpc();
    }

    public void StopBoostVFX()
    {
        StopBoostVFXLocal();
        if (ShouldNetwork()) StopBoostVFXClientRpc();
    }

    public void PlayAirTrickVFX(bool isLeft)
    {
        PlayAirTrickVFXLocal(isLeft);
        if (ShouldNetwork()) PlayAirTrickVFXClientRpc(isLeft);
    }

    public void ParticleSystemsL()
    {
        ParticleSystemsL_Local();
        if (ShouldNetwork()) ParticleSystemsL_ClientRpc();
    }

    public void ParticleSystemsR()
    {
        ParticleSystemsR_Local();
        if (ShouldNetwork()) ParticleSystemsR_ClientRpc();
    }

    public void TireScreechesL()
    {
        TireScreechesL_Local();
        if (ShouldNetwork()) TireScreechesL_ClientRpc();
    }

    public void TireScreechesR()
    {
        TireScreechesR_Local();
        if (ShouldNetwork()) TireScreechesR_ClientRpc();
    }

    public void TierTransitionSparksL()
    {
        TierTransitionSparksL_Local();
        if (ShouldNetwork()) TierTransitionSparksL_ClientRpc();
    }

    public void TierTransitionSparksR()
    {
        TierTransitionSparksR_Local();
        if (ShouldNetwork()) TierTransitionSparksR_ClientRpc();
    }

    public void ColorDrift(Color c)
    {
        ColorDriftLocal(c);
        if (ShouldNetwork()) ColorDriftClientRpc(c);
    }

    public void StopAllParticles()
    {
        StopParticles(particleSystemsBR);
        StopParticles(particleSystemsBL);
        StopParticles(TireScreechesLtoR);
        StopParticles(transitionSparksLtoR);
        StopParticles(boostFlames);
        if (airTrickPS != null) airTrickPS.Stop();
    }

    public void StopDriftVFX()
    {
        StopDriftVFXLocal();
        if (ShouldNetwork()) StopDriftVFXClientRpc();
    }

    // ---------- Local Methods ----------

    void PlayBoostVFXLocal()
    {
        if (boostFlames != null)
        {
            foreach (var ps in boostFlames)
                if (ps != null && !ps.isPlaying) ps.Play();
        }
    }

    void StopBoostVFXLocal()
    {
        if (boostFlames != null)
        {
            foreach (var ps in boostFlames)
                if (ps != null && ps.isPlaying) ps.Stop();
        }
    }

    void PlayAirTrickVFXLocal(bool isLeft)
    {
        if (airTrickPS != null)
        {
            var main = airTrickPS.main;
            main.flipRotation = isLeft ? 1 : 0;
            airTrickPS.Play();
        }
    }

    void ParticleSystemsL_Local()
    {
        foreach (var ps in particleSystemsBL)
        {
            if (!ps.isPlaying)
                ps.Play();
        }

        foreach (var ps in particleSystemsBR)
        {
            if (ps.isPlaying)
                ps.Stop();
        }
    }

    void ParticleSystemsR_Local()
    {
        foreach (var ps in particleSystemsBR)
        {
            if (!ps.isPlaying)
                ps.Play();
        }

        foreach (var ps in particleSystemsBL)
        {
            if (ps.isPlaying)
                ps.Stop();
        }
    }


    void TireScreechesL_Local()
    {
        if (TireScreechesLtoR != null && TireScreechesLtoR.Count >= 4)
        {
            if (TireScreechesLtoR[0] != null && !TireScreechesLtoR[0].isPlaying)
            {
                TireScreechesLtoR[0]?.Play();
                TireScreechesLtoR[2]?.Play();

                if (TireScreechesLtoR[1]?.isPlaying == true)
                {
                    TireScreechesLtoR[1]?.Stop();
                    TireScreechesLtoR[3]?.Stop();
                    StopParticles(particleSystemsBR);
                }
            }
        }
    }

    void TireScreechesR_Local()
    {
        if (TireScreechesLtoR != null && TireScreechesLtoR.Count >= 4)
        {
            if (TireScreechesLtoR[1] != null && !TireScreechesLtoR[1].isPlaying)
            {
                TireScreechesLtoR[1]?.Play();
                TireScreechesLtoR[3]?.Play();

                if (TireScreechesLtoR[0]?.isPlaying == true)
                {
                    TireScreechesLtoR[0]?.Stop();
                    TireScreechesLtoR[2]?.Stop();
                    StopParticles(particleSystemsBR);
                }
            }
        }
    }

    void TierTransitionSparksL_Local()
    {
        if (transitionSparksLtoR != null && transitionSparksLtoR.Count >= 6)
        {
            transitionSparksLtoR[0]?.Play();
            transitionSparksLtoR[2]?.Play();
            transitionSparksLtoR[4]?.Play();
            transitionSparksLtoR[6]?.Play();
        }
    }

    void TierTransitionSparksR_Local()
    {
        if (transitionSparksLtoR != null && transitionSparksLtoR.Count >= 7)
        {
            transitionSparksLtoR[1]?.Play();
            transitionSparksLtoR[3]?.Play();
            transitionSparksLtoR[5]?.Play();
            transitionSparksLtoR[7]?.Play();
        }
    }

    void ColorDriftLocal(Color c)
    {
        void SetColor(List<ParticleSystem> list)
        {
            if (list == null) return;

            foreach (var ps in list)
            {
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = c;
                }
            }
        }

        SetColor(particleSystemsBL);
        SetColor(particleSystemsBR);
        SetColor(TireScreechesLtoR);
        SetColor(transitionSparksLtoR);
    }

    void StopDriftVFXLocal()
    {
        StopParticles(particleSystemsBL);
        StopParticles(particleSystemsBR);
        StopParticles(TireScreechesLtoR);
        StopParticles(transitionSparksLtoR);
    }

    void PlayParticles(List<ParticleSystem> list)
    {
        if (list == null) return;

        foreach (var ps in list)
        {
            if (ps != null && !ps.isPlaying)
                ps.Play();
        }
    }

    void StopParticles(List<ParticleSystem> list)
    {
        if (list == null) return;

        foreach (var ps in list)
        {
            if (ps != null && ps.isPlaying)
                ps.Stop();
        }
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
