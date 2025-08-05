using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;

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

    [Header("Item Effects")]
    public VisualEffect shield;
    public VisualEffect itemBoost;
    public List<VisualEffect> hover;
    public VisualEffect warpBoostBottom;
    public VisualEffect warpBoostTop;


    public List<ParticleSystem> boostFlames;

    // temporary fix to stop warp boost when loaded into the scene (can be deleted)
    private void Awake()
    {
        warpBoostBottom.Stop();
        warpBoostTop.Stop();
    }

    // ---------- Public Methods ----------
    public void StopItemEffects()
    {
        StopItemEffectsLocal();

        if (IsOwner)
            StopItemEffectsServerRpc();
    }

    public void PlayBoostVFX()
    {
        PlayBoostVFXLocal();

        if (IsOwner)
            PlayBoostVFXServerRpc();
    }

    public void StopBoostVFX()
    {
        StopBoostVFXLocal();

        if (IsOwner)
            StopBoostVFXServerRpc();
    }

    public void PlayAirTrickVFX(bool isLeft)
    {
        PlayAirTrickVFXLocal(isLeft);

        if (IsOwner)
            PlayAirTrickVFXServerRpc(isLeft);
    }

    public void ParticleSystemsL()
    {
        ParticleSystemsL_Local();

        if (IsOwner)
            ParticleSystemsL_ServerRpc();
    }

    public void ParticleSystemsR()
    {
        ParticleSystemsR_Local();

        if (IsOwner)
            ParticleSystemsR_ServerRpc();
    }

    public void TireScreechesL()
    {
        TireScreechesL_Local();

        if (IsOwner)
            TireScreechesL_ServerRpc();
    }

    public void TireScreechesR()
    {
        TireScreechesR_Local();

        if (IsOwner)
            TireScreechesR_ServerRpc();
    }

    public void TierTransitionSparksL()
    {
        TierTransitionSparksL_Local();

        if (IsOwner)
            TierTransitionSparksL_ServerRpc();
    }

    public void TierTransitionSparksR()
    {
        TierTransitionSparksR_Local();

        if (IsOwner)
            TierTransitionSparksR_ServerRpc();
    }

    public void ColorDrift(Color c)
    {
        ColorDriftLocal(c);

        if (IsOwner)
            ColorDriftServerRpc(c);
    }

    public void StopAllParticles()
    {
        particleSystemsBR?.ForEach(ps => ps?.Stop());
        particleSystemsBL?.ForEach(ps => ps?.Stop());
        TireScreechesLtoR?.ForEach(ps => ps?.Stop());
        transitionSparksLtoR?.ForEach(ps => ps?.Stop());
        boostFlames?.ForEach(ps => ps?.Stop());
        airTrickPS?.Stop();
    }

    public void StopDriftVFX()
    {
        StopDriftVFXLocal();

        if (IsOwner)
            StopDriftVFXServerRpc();
    }

    public void PlayShieldVFX(float duration)
    {
        if (!IsSpawned)
        {
            PlayShieldVFXLocal(duration);
        }
        else
        {
            PlayShieldVFXServerRpc(duration);
        }
    }

    public void PlayItemBoostVFX(float duration)
    {
        PlayItemBoostVFXLocal(duration);
        if (IsOwner)
            PlayItemBoostVFXServerRpc(duration);
    }

    public void PlayHoverVFX(float duration)
    {
        PlayHoverVFXLocal(duration);
        if (IsOwner)
            PlayHoverVFXServerRpc(duration);
    }


    // ---------- Local Methods ----------

    void PlayBoostVFXLocal()
    {
        boostFlames?.ForEach(ps => { if (ps != null && !ps.isPlaying) ps.Play(); });
    }

    void StopBoostVFXLocal()
    {
        boostFlames?.ForEach(ps => { if (ps != null && ps.isPlaying) ps.Stop(); });
    }

    void PlayAirTrickVFXLocal(bool isLeft)
    {
        if (airTrickPS == null) return;
        var main = airTrickPS.main;
        main.flipRotation = isLeft ? 1 : 0;
        airTrickPS.Play();
    }

    void ParticleSystemsL_Local()
    {
        particleSystemsBL?.ForEach(ps => { if (ps != null && !ps.isPlaying) ps.Play(); });
        particleSystemsBR?.ForEach(ps => ps?.Stop());
    }

    void ParticleSystemsR_Local()
    {
        particleSystemsBR?.ForEach(ps => { if (ps != null && !ps.isPlaying) ps.Play(); });
        particleSystemsBL?.ForEach(ps => ps?.Stop());
    }

    void TireScreechesL_Local()
    {
        if (TireScreechesLtoR == null || TireScreechesLtoR.Count < 4) return;

        if (!TireScreechesLtoR[0].isPlaying)
        {
            TireScreechesLtoR[0].Play();
            TireScreechesLtoR[2].Play();

            if (TireScreechesLtoR[1].isPlaying)
            {
                TireScreechesLtoR[1].Stop();
                TireScreechesLtoR[3].Stop();
                particleSystemsBR?.ForEach(ps => ps?.Stop());
            }
        }
    }

    void TireScreechesR_Local()
    {
        if (TireScreechesLtoR == null || TireScreechesLtoR.Count < 4) return;

        if (!TireScreechesLtoR[1].isPlaying)
        {
            TireScreechesLtoR[1].Play();
            TireScreechesLtoR[3].Play();

            if (TireScreechesLtoR[0].isPlaying)
            {
                TireScreechesLtoR[0].Stop();
                TireScreechesLtoR[2].Stop();
                particleSystemsBL?.ForEach(ps => ps?.Stop());
            }
        }
    }

    void TierTransitionSparksL_Local()
    {
        if (transitionSparksLtoR == null || transitionSparksLtoR.Count < 6) return;

        transitionSparksLtoR[0].Play();
        transitionSparksLtoR[2].Play();
        transitionSparksLtoR[4].Play();
        transitionSparksLtoR[5].Play();
    }

    void TierTransitionSparksR_Local()
    {
        if (transitionSparksLtoR == null || transitionSparksLtoR.Count < 7) return;

        transitionSparksLtoR[1].Play();
        transitionSparksLtoR[3].Play();
        transitionSparksLtoR[5].Play();
        transitionSparksLtoR[6].Play();
    }

    void ColorDriftLocal(Color c)
    {
        void SetColor(ParticleSystem ps)
        {
            if (ps == null) return;
            var main = ps.main;
            main.startColor = c;
        }

        particleSystemsBL?.ForEach(SetColor);
        particleSystemsBR?.ForEach(SetColor);
        TireScreechesLtoR?.ForEach(SetColor);
        transitionSparksLtoR?.ForEach(SetColor);
    }

    void StopDriftVFXLocal()
    {
        particleSystemsBL?.ForEach(ps => ps?.Stop());
        particleSystemsBR?.ForEach(ps => ps?.Stop());
        TireScreechesLtoR?.ForEach(ps => ps?.Stop());
        transitionSparksLtoR?.ForEach(ps => ps?.Stop());
    }

    void StopItemEffectsLocal()
    {
        if (shield != null && shield.isActiveAndEnabled)
            shield.Stop();
        if (itemBoost != null && itemBoost.isActiveAndEnabled)
            itemBoost.Stop();
        if (hover != null)
            hover.ForEach(vfx => { if (vfx.isActiveAndEnabled) vfx.Stop(); });
        if (warpBoostBottom != null && warpBoostBottom.isActiveAndEnabled)
            warpBoostBottom.Stop();
    }

    void PlayShieldVFXLocal(float duration)
    {
        if (shield == null) return;
        shield.SetFloat("Duration", duration);
        shield.Play();
    }

    void PlayItemBoostVFXLocal(float duration)
    {
        if (itemBoost == null) return;
        //itemBoost.SetFloat("Duration", duration);
        itemBoost.Play();
    }

    void PlayHoverVFXLocal(float duration)
    {
        if (hover == null) return;

        foreach (var vfx in hover)
        {
            if (vfx != null)
            {
                vfx.SetFloat("Duration", duration);
                vfx.Play();
            }
        }
    }


    // ---------- ServerRPCs ----------

    [ServerRpc]
    void PlayBoostVFXServerRpc() => PlayBoostVFXClientRpc();

    [ServerRpc]
    void StopBoostVFXServerRpc() => StopBoostVFXClientRpc();

    [ServerRpc]
    void PlayAirTrickVFXServerRpc(bool isLeft) => PlayAirTrickVFXClientRpc(isLeft);

    [ServerRpc]
    void ParticleSystemsL_ServerRpc() => ParticleSystemsL_ClientRpc();

    [ServerRpc]
    void ParticleSystemsR_ServerRpc() => ParticleSystemsR_ClientRpc();

    [ServerRpc]
    void TireScreechesL_ServerRpc() => TireScreechesL_ClientRpc();

    [ServerRpc]
    void TireScreechesR_ServerRpc() => TireScreechesR_ClientRpc();

    [ServerRpc]
    void TierTransitionSparksL_ServerRpc() => TierTransitionSparksL_ClientRpc();

    [ServerRpc]
    void TierTransitionSparksR_ServerRpc() => TierTransitionSparksR_ClientRpc();

    [ServerRpc]
    void ColorDriftServerRpc(Color c) => ColorDriftClientRpc(c);

    [ServerRpc]
    void StopDriftVFXServerRpc() => StopDriftVFXClientRpc();

    [ServerRpc]
    void StopItemEffectsServerRpc() => StopItemEffectsClientRpc();

    [Rpc(SendTo.Server)]
    void PlayShieldVFXServerRpc(float duration) => PlayShieldVFXClientRpc(duration);

    [ServerRpc]
    void PlayItemBoostVFXServerRpc(float duration) => PlayItemBoostVFXClientRpc(duration);

    [ServerRpc]
    void PlayHoverVFXServerRpc(float duration) => PlayHoverVFXClientRpc(duration);


    // ---------- ClientRPCs ----------

    [ClientRpc]
    void PlayBoostVFXClientRpc() { if (!IsOwner) PlayBoostVFXLocal(); }

    [ClientRpc]
    void StopBoostVFXClientRpc() { if (!IsOwner) StopBoostVFXLocal(); }

    [ClientRpc]
    void PlayAirTrickVFXClientRpc(bool isLeft) { if (!IsOwner) PlayAirTrickVFXLocal(isLeft); }

    [ClientRpc]
    void ParticleSystemsL_ClientRpc() { if (!IsOwner) ParticleSystemsL_Local(); }

    [ClientRpc]
    void ParticleSystemsR_ClientRpc() { if (!IsOwner) ParticleSystemsR_Local(); }

    [ClientRpc]
    void TireScreechesL_ClientRpc() { if (!IsOwner) TireScreechesL_Local(); }

    [ClientRpc]
    void TireScreechesR_ClientRpc() { if (!IsOwner) TireScreechesR_Local(); }

    [ClientRpc]
    void TierTransitionSparksL_ClientRpc() { if (!IsOwner) TierTransitionSparksL_Local(); }

    [ClientRpc]
    void TierTransitionSparksR_ClientRpc() { if (!IsOwner) TierTransitionSparksR_Local(); }

    [ClientRpc]
    void ColorDriftClientRpc(Color c) { if (!IsOwner) ColorDriftLocal(c); }

    [ClientRpc]
    void StopDriftVFXClientRpc() { if (!IsOwner) StopDriftVFXLocal(); }

    [ClientRpc]
    void StopItemEffectsClientRpc() { if (!IsOwner) StopItemEffectsLocal(); }

    [Rpc(SendTo.ClientsAndHost)]
    void PlayShieldVFXClientRpc(float duration) { PlayShieldVFXLocal(duration); }

    [ClientRpc]
    void PlayItemBoostVFXClientRpc(float duration) { if (!IsOwner) PlayItemBoostVFXLocal(duration); }

    [ClientRpc]
    void PlayHoverVFXClientRpc(float duration) { if (!IsOwner) PlayHoverVFXLocal(duration); }

}

