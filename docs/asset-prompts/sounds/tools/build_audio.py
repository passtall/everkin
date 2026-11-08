from pathlib import Path
import json, csv, hashlib, zipfile, subprocess
import numpy as np
from scipy import signal
import soundfile as sf
import pyloudnorm as pyln
import imageio_ffmpeg

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT if ROOT.name == 'Everkin_Sound_Pack' else ROOT / 'Everkin_Sound_Pack'
SR = 48000
R = np.random.default_rng(8172026)
meter = pyln.Meter(SR)
records = []
FFMPEG = imageio_ffmpeg.get_ffmpeg_exe()

def encode(path,x):
    channels=1 if x.ndim==1 else x.shape[1]
    args=[FFMPEG,'-hide_banner','-loglevel','error','-y','-f','f32le','-ar',str(SR),'-ac',str(channels),'-i','pipe:0','-c:a','libvorbis','-q:a','5',str(path)]
    subprocess.run(args,input=x.astype('<f4').tobytes(),check=True,capture_output=True)

def noise(n, lo=150, hi=6000):
    x = R.normal(0, 1, n)
    sos = signal.butter(2, [lo, min(hi, 22000)], btype='bandpass', fs=SR, output='sos')
    x = signal.sosfilt(sos, x)
    return x / (np.std(x) + 1e-9)

def env(n, attack=.006, decay=7):
    t = np.arange(n) / SR
    return (1-np.exp(-t/max(attack,.0001))) * np.exp(-decay*t/(n/SR))

def tone(d, f=500, end=None, decay=7, airy=0):
    n = round(d*SR); t=np.arange(n)/SR
    freq=np.linspace(f, f if end is None else end,n)
    phase=2*np.pi*np.cumsum(freq)/SR
    x=np.sin(phase)+.25*np.sin(phase*1.47)+.1*np.sin(phase*2.31)
    return (x+airy*noise(n,800,8000))*env(n, .004,decay)

def put(x,y,at=0,gain=1):
    i=round(at*SR); end=min(len(x),i+len(y))
    if end>i: x[i:end]+=gain*y[:end-i]

def hit(d=.2, f=230, bright=False):
    n=round(d*SR)
    return .7*tone(d,f,f*.65,9)+.22*noise(n,700 if bright else 140,7000 if bright else 2400)*env(n,.001,13)

def swish(d, up=True):
    n=round(d*SR); t=np.linspace(0,1,n)
    e=np.sin(np.pi*t)**1.7
    a=noise(n,250,1700); b=noise(n,2500,9000)
    blend=t if up else 1-t
    return .4*((1-blend)*a+blend*b)*e

def sparkle(d, rising=True, count=7):
    x=np.zeros(round(d*SR))
    for i in range(count):
        at=d*.65*i/max(count-1,1)
        f=R.uniform(1100,3100)*( .7+.6*i/count if rising else 1.3-.6*i/count)
        put(x,tone(min(.19,d-at),f,decay=6),at,.16)
    return x

def effect(name,d,v=0):
    x=np.zeros(round(d*SR)); p=R.uniform(.92,1.08)
    if name in ('confirm','cancel','bump','melee_hit','projectile_hit','drag_drop','invalid'):
        put(x,hit(d, (330 if name=='confirm' else 170)*p),0,.7)
        if name=='confirm': put(x,tone(.14,1650*p),.025,.13)
        if name=='invalid': put(x,hit(.12,145*p),.11,.65)
        if name in ('melee_hit','projectile_hit'): x+=sparkle(d,count=4)*.4
        if name=='drag_drop': x+=swish(d,False)*.22
    elif name in ('panel_open','panel_close','tab_switch','drag_pickup','card_flip','projectile_launch','stealth_on','stealth_off','ko_thud','intercept'):
        x+=swish(d,name not in ('panel_close','stealth_on','ko_thud'))
        if name in ('card_flip','tab_switch'): put(x,hit(.07,500*p,True),d*.5,.22)
        if name in ('ko_thud','intercept'): put(x,hit(d*.35,210*p),d*.62,.7)
    elif name=='hover': x+=noise(len(x),2500,9000)*env(len(x),.002,10)*.15
    elif name=='coins':
        for at in (.01,.09,.17,.28): put(x,tone(.2,R.uniform(2100,3700),decay=9),at,.25)
        x+=swish(d,False)*.1
    elif name in ('magic_cast','magic_hit','heal','status_helpful','momentum_threshold','momentum_max','victory'):
        x+=sparkle(d,True,10 if d>.7 else 6)
        x+=swish(d)*.3
        if name in ('heal','victory'): x+=tone(d,520*p,610*p,2,.05)*.1
        if name=='magic_hit': put(x,hit(.18,400*p),0,.2)
    elif name in ('status_harmful','defeat'):
        t=np.arange(len(x))/SR
        x+=tone(d,300*p,100*p,3,.05)*.4
        x+=np.sin(2*np.pi*170*p*t+1.4*np.sin(2*np.pi*9*t))*env(len(x),.02,5)*.15
    elif name in ('status_end','ground_poison','ground_water'):
        count=1 if name=='status_end' else 6
        for i in range(count):
            at=R.uniform(.01,d*.65); dur=min(.18,d-at)
            put(x,tone(dur,R.uniform(350,800),R.uniform(100,220),8),at,.35)
        if name=='ground_water': x+=noise(len(x),500,7000)*env(len(x),.008,6)*.13
    elif name=='shield_absorb':
        x+=tone(d,650*p,420*p,5)*.4
        put(x,hit(.15,150*p),.025,.5)
    elif name=='turn_chime': x+=tone(d,1350*p,decay=7)*.45
    elif name in ('summon_appear','ground_ice','ko_crack'):
        for i in range(13 if name=='summon_appear' else 7):
            at=R.uniform(.01,d*.8)
            put(x,hit(min(.09,d-at),R.uniform(1100,2600),True),at,R.uniform(.08,.17))
        if name=='summon_appear': x+=swish(d)*.16
    elif name in ('ground_fire','ground_steam'):
        x+=noise(len(x),180 if name=='ground_fire' else 1200,6500)*env(len(x),.025,4)*.25
        if name=='ground_fire':
            for at in (.02,.13,.31,.42): put(x,hit(.06,500,True),at,.1)
    else: raise ValueError(name)
    fade=min(round(.015*SR),len(x)//5)
    x[:fade]*=np.linspace(0,1,fade); x[-fade:]*=np.linspace(1,0,fade)
    return x

def loudness(x):
    # EBU R128's 400 ms gate cannot measure very short one-shots.
    # Repeat short files for a stable measurement without changing the delivered duration.
    y=x if len(x)>=SR*3 else np.tile(x,(int(np.ceil(SR*3/len(x))),)+(1,)*(x.ndim-1))
    return float(meter.integrated_loudness(y))

def truepeak(x):
    return float(np.max(np.abs(signal.resample_poly(x,4,1,axis=0))))

def save(rel,x,target,loop=False):
    path=OUT/rel; path.parent.mkdir(parents=True,exist_ok=True)
    x=x-np.mean(x,axis=0)
    x*=10**((target-loudness(x))/20)
    # Gentle saturation reduces outliers; preserve transients wherever possible.
    if truepeak(x)>.84:
        x=.75*np.tanh(x/.75)
        x*=10**((target-loudness(x))/20)
    peak=truepeak(x)
    if peak>.84: x*=.84/peak
    for _ in range(5):
        encode(path,x)
        y,rate=sf.read(path,always_2d=False)
        pk=truepeak(y); lu=loudness(y)
        correction=min(target-lu,20*np.log10(.84/max(pk,1e-12)))
        if abs(correction)<.04: break
        x*=10**(correction/20)
    row={'file':rel,'duration_s':len(y)/rate,'sample_rate':rate,'channels':1 if y.ndim==1 else y.shape[1],
         'lufs':round(lu,2),'target_lufs':target,'true_peak_dbtp':round(20*np.log10(pk),2),
         'loop':loop,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()}
    if loop:
        row['join_step_dbfs']=round(20*np.log10(max(float(np.max(abs(y[-1]-y[0]))),1e-12)),2)
        row['join_step_vs_internal_p999']=round(float(np.max(abs(y[-1]-y[0])))/max(float(np.quantile(abs(np.diff(y,axis=0)),.999)),1e-12),3)
    records.append(row)
    print(f'{rel}: {lu:.1f} LUFS, {row["true_peak_dbtp"]:.1f} dBTP',flush=True)
    return y

UI={'confirm':.2,'cancel':.2,'invalid':.25,'panel_open':.3,'panel_close':.3,'hover':.08,'tab_switch':.15,'drag_pickup':.15,'drag_drop':.2,'card_flip':.3,'coins':.5}
BATTLE={'melee_hit':.3,'projectile_launch':.25,'projectile_hit':.3,'magic_cast':.5,'magic_hit':.4,'heal':.7,'status_helpful':.5,'status_harmful':.5,'status_end':.4,'intercept':.35,'shield_absorb':.35,'bump':.25,'stealth_on':.5,'stealth_off':.4,'summon_appear':.8,'ground_fire':.6,'ground_ice':.5,'ground_water':.5,'ground_steam':.6,'ground_poison':.6,'turn_chime':.4,'momentum_threshold':.5,'momentum_max':.8,'ko_crack':.4,'ko_thud':.4}

def periodic_noise(n,lo,hi):
    # Circular FFT filtering gives a continuous waveform across the wrap point.
    f=np.fft.rfftfreq(n,1/SR)
    spectrum=np.fft.rfft(R.normal(size=n))
    shape=(f/lo)**2/(1+(f/lo)**2) / (1+(f/hi)**4)
    spectrum*=shape
    x=np.fft.irfft(spectrum,n=n)
    return x/(np.std(x)+1e-9)

def ambience(name,d=60):
    n=SR*d; t=np.arange(n)/SR; x=np.zeros((n,2))
    windy=name in ('desert','mountain','savannah','arctic','storm_cliffs','ashlands')
    watery=name in ('wet_forest','swamp','coast','lagoon','storm_cliffs')
    interior=name in ('cathedral','crystal_caverns','mushroom_grotto')
    lo,hi=(45,1200) if windy else (180,2400)
    if name=='arctic': lo,hi=350,4000
    if interior: lo,hi=45,500
    for c in range(2):
        bed=periodic_noise(n,lo,hi)
        modulation=.65+.18*np.sin(2*np.pi*3*t/d+c)+.12*np.sin(2*np.pi*7*t/d+.8*c)
        x[:,c]=bed*modulation*.055
        if watery:
            water=periodic_noise(n,400,6500)
            wave=(.5+.5*np.sin(2*np.pi*(9 if name in ('coast','storm_cliffs') else 17)*t/d+c*.2))**2
            x[:,c]+=water*wave*.055
    def event(y,at,gain=.1):
        pan=R.uniform(.1,.9); i=round(at*SR)%n; ix=(np.arange(len(y))+i)%n
        for c,g in enumerate((np.sqrt(1-pan),np.sqrt(pan))):
            np.add.at(x[:,c],ix,y*gain*g)
    insects=name in ('pine_forest','jungle','wet_forest','desert','savannah','swamp','ruins','autumn_woods','flower_meadow')
    if insects:
        for _ in range(100 if name in ('jungle','swamp') else 55):
            at=R.uniform(0,d); f=R.uniform(2800,4900)
            chirp=tone(R.uniform(.06,.15),f,f*1.035,4)
            event(chirp,at,.012)
            if R.random()<.6: event(chirp,at+.14,.009)
    if name in ('jungle','wet_forest','swamp'):
        for _ in range(22):
            dur=R.uniform(.2,.55); tt=np.arange(round(dur*SR))/SR
            frog=tone(dur,R.uniform(250,600),R.uniform(150,250),4)*( .65+.35*np.sin(2*np.pi*24*tt))
            event(frog,R.uniform(0,d),.035)
    if name in ('pine_forest','autumn_woods','desert','savannah','lagoon'):
        for at in (13.2,38.8):
            dur=1.3; tt=np.arange(round(dur*SR))/SR
            f=420 if name in ('pine_forest','autumn_woods') else 650
            call=np.sin(2*np.pi*(f*tt+25*np.sin(tt*3)))*np.sin(np.pi*tt/dur)**3
            event(call,at,.018)
    if name in ('jungle','wet_forest','crystal_caverns','mushroom_grotto','lagoon'):
        for _ in range(28): event(tone(.22,R.uniform(800,1800),350,8),R.uniform(0,d),.022)
    if name in ('mountain','ashlands','arctic','cathedral','ruins','autumn_woods'):
        for _ in range(16):
            if name in ('arctic','cathedral'):
                y=tone(.8,R.uniform(95,200),R.uniform(70,120),4,.025)
            else: y=hit(.15,R.uniform(250,700),True)
            event(y,R.uniform(0,d),.018)
    if name in ('crystal_caverns','mushroom_grotto','ashlands'):
        for c in range(2):
            f=73 if name=='ashlands' else 131
            x[:,c]+=.008*np.sin(2*np.pi*f*t)*( .7+.3*np.sin(2*np.pi*2*t/d+c*.2))
    if name=='storm_cliffs':
        for at in (9,39):
            nn=SR*7; tt=np.arange(nn)/SR
            thunder=noise(nn,25,260)*np.sin(np.pi*tt/7)**2
            event(thunder,at,.045)
    return x

def main():
    OUT.mkdir(exist_ok=True)
    previews=[]
    for name,d in UI.items():
        y=save(f'audio/ui/{name}.ogg',effect(name,d),-16)
        if name in ('confirm','panel_open','coins'): previews.append((name,y))
    for name,d in BATTLE.items():
        for v in range(1,4):
            y=save(f'audio/battle/{name}_{v}.ogg',effect(name,d,v),-16)
            if v==1 and name in ('melee_hit','magic_cast','heal','ground_water','summon_appear'): previews.append((name,y))
    for name in ('victory','defeat'):
        y=save(f'audio/result/{name}.ogg',effect(name,2),-16)
        previews.append((name,y))
    biomes=['pine_forest','jungle','wet_forest','desert','mountain','cathedral','savannah','arctic','swamp','coast','ashlands','crystal_caverns','mushroom_grotto','lagoon','ruins','autumn_woods','storm_cliffs','flower_meadow']
    for name in biomes: save(f'audio/ambient/{name}.ogg',ambience(name),-23,True)
    demo=[]; cues=[]; cursor=0
    for name,y in previews:
        cues.append({'time_s':round(cursor/SR,2),'sound':name})
        demo.extend((y,np.zeros(round(.45*SR)))); cursor+=len(y)+round(.45*SR)
    sf.write(OUT/'preview_effects.wav',np.concatenate(demo),SR,subtype='PCM_16')
    a,_=sf.read(OUT/'audio/ambient/pine_forest.ogg')
    b,_=sf.read(OUT/'audio/ambient/crystal_caverns.ogg')
    sf.write(OUT/'preview_ambient.wav',np.concatenate((a[:SR*8],np.zeros((SR,2)),b[:SR*8])),SR,subtype='PCM_16')
    (OUT/'preview_cues.json').write_text(json.dumps(cues,indent=2))
    (OUT/'manifest.json').write_text(json.dumps(records,indent=2))
    with (OUT/'manifest.csv').open('w',newline='') as f:
        keys=list(dict.fromkeys(k for r in records for k in r))
        writer=csv.DictWriter(f,fieldnames=keys); writer.writeheader(); writer.writerows(records)
    assert len(records)==106
    assert all(r['sample_rate']==48000 and r['true_peak_dbtp']<=-.9 for r in records)
    assert all(r['channels']==(2 if r['loop'] else 1) for r in records)
    print('Completed 106 assets.',flush=True)

if __name__=='__main__': main()
