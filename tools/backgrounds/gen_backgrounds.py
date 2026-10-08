"""Generate Everkin biome backgrounds as SVG.

Two styles from the same scene description:
  card   - 1024x1536, faceted low-poly look matching the creature art
  battle - 1920x1080, smooth, layered, glowing night scene

Usage: python3 gen_backgrounds.py OUT_DIR [biome_id ...]
Writes OUT_DIR/card/<id>.svg and OUT_DIR/battle/<id>.svg. Stdlib only.
"""
import math, os, random, sys

# ---------- colour helpers ----------
def hx(c):
    c = c.lstrip('#'); return tuple(int(c[i:i+2], 16) for i in (0, 2, 4))
def tohex(t): return '#%02x%02x%02x' % tuple(max(0, min(255, int(round(v)))) for v in t)
def mix(a, b, t):
    a, b = hx(a), hx(b); return tohex(tuple(a[i] + (b[i]-a[i])*t for i in range(3)))
def shade(c, f):
    c = hx(c); return tohex(tuple(v*f for v in c))
def lighten(c, t): return mix(c, '#ffffff', t)

# ---------- noise ----------
class Noise1D:
    def __init__(self, rnd, n=64):
        self.v = [rnd.random() for _ in range(n)]; self.n = n
    def at(self, x):
        i = math.floor(x); f = x - i; f = f*f*(3-2*f)
        a = self.v[i % self.n]; b = self.v[(i+1) % self.n]
        return a + (b-a)*f
    def fbm(self, x, oct=4):
        s, amp, fr, tot = 0, 1, 1, 0
        for _ in range(oct):
            s += amp*self.at(x*fr); tot += amp; amp *= .5; fr *= 2.03
        return s/tot

# ---------- shapes ----------
# A shape is dict(pts=[(x,y)...], color, glow=None, kind)
def poly(pts, color, glow=None, kind='solid'):
    return dict(pts=pts, color=color, glow=glow, kind=kind)

def circle_pts(cx, cy, rx, ry, n, rnd=None, jit=0.0, a0=0.0, a1=2*math.pi):
    out = []
    for i in range(n):
        a = a0 + (a1-a0)*i/(n if a1-a0 >= 2*math.pi-1e-6 else n-1)
        j = 1 + (rnd.uniform(-jit, jit) if rnd else 0)
        out.append((cx + math.cos(a)*rx*j, cy + math.sin(a)*ry*j))
    return out

def quad_seg(x0, y0, x1, y1, w0, w1):
    dx, dy = x1-x0, y1-y0; L = math.hypot(dx, dy) or 1
    nx, ny = -dy/L, dx/L
    return [(x0+nx*w0/2, y0+ny*w0/2), (x1+nx*w1/2, y1+ny*w1/2),
            (x1-nx*w1/2, y1-ny*w1/2), (x0-nx*w0/2, y0-ny*w0/2)]

# ---------- props: each returns list of shapes, (x, ground y, height h) ----------
def p_pine(r, x, y, h, c, **k):
    out = [poly(quad_seg(x, y, x, y-h*.25, h*.06, h*.05), shade(c, .8))]
    tiers = k.get('tiers', 4)
    for t in range(tiers):
        tb = y - h*.15 - h*.8*t/tiers; tt = tb - h*.8/tiers*1.7
        w = h*.42*(1 - t/(tiers+.6))
        th = tb - tt; tx = x + r.uniform(-.03, .03)*h
        out.append(poly([(x-w/2, tb + th*.08), (x-w*.32, tb - th*.04), (x-w*.18, tb - th*.22), (tx, tt),
                         (x+w*.2, tb - th*.24), (x+w*.34, tb - th*.05), (x+w/2, tb + th*.06), (x, tb - th*.06)], c))
    return out

def p_dead(r, x, y, h, c, depth=4, moss=None, **k):
    out = []
    def br(x0, y0, ang, L, w, d):
        x1 = x0 + math.cos(ang)*L; y1 = y0 - math.sin(ang)*L
        out.append(poly(quad_seg(x0, y0, x1, y1, w, w*.62), c))
        if moss and d <= 2 and r.random() < .7:
            mh = L*r.uniform(.6, 1.4)
            out.append(poly([(x1-w, y1), (x1+w, y1), (x1+w*.3, y1+mh), (x1-w*.4, y1+mh*.8)], moss))
        if d > 0:
            for s in (-1, 1):
                if r.random() < .85:
                    br(x1, y1, ang + s*r.uniform(.3, .75), L*r.uniform(.55, .75), w*.62, d-1)
    br(x, y, math.pi/2 + r.uniform(-.12, .12), h*.42, h*.07, depth)
    return out

def p_broad(r, x, y, h, c, crown=None, **k):
    crown = crown or c
    out = [poly(quad_seg(x, y, x+r.uniform(-.05, .05)*h, y-h*.55, h*.08, h*.05), shade(c, .85))]
    for i in range(3):
        cx = x + r.uniform(-.22, .22)*h; cy = y - h*.62 - r.uniform(0, .22)*h
        rr = h*r.uniform(.22, .32)
        out.append(poly(circle_pts(cx, cy, rr, rr*.85, 9, r, .12, r.random()), crown))
    return out

def p_palm(r, x, y, h, c, **k):
    out = []; lean = r.uniform(-.25, .25)*h
    tx, ty = x+lean, y-h
    pts = [(x, y)]
    for i in range(1, 6):
        t = i/5; pts.append((x + lean*t*t, y - h*t))
    for a, b in zip(pts, pts[1:]):
        out.append(poly(quad_seg(a[0], a[1], b[0], b[1], h*.05, h*.04), shade(c, .9)))
    for i in range(7):
        ang = math.pi*(.05 + .9*i/6) + r.uniform(-.1, .1); L = h*r.uniform(.35, .5)
        mx, my = tx + math.cos(ang)*L*.55, ty - math.sin(ang)*L*.35
        ex, ey = tx + math.cos(ang)*L, ty - math.sin(ang)*L*.1 + L*.35
        out.append(poly([(tx, ty), (mx, my-h*.03), (ex, ey), (mx, my+h*.03)], c))
    return out

def p_jungle(r, x, y, h, c, **k):
    out = p_broad(r, x, y, h*1.25, c)
    for i in range(r.randint(1, 3)):
        vx = x + r.uniform(-.25, .25)*h; vy = y - h*.7
        out.append(poly(quad_seg(vx, vy, vx+r.uniform(-8, 8), vy + h*r.uniform(.3, .6), 3, 2), c))
    return out

def p_acacia(r, x, y, h, c, **k):
    out = [poly(quad_seg(x, y, x+h*.05, y-h*.55, h*.07, h*.05), c)]
    out.append(poly(quad_seg(x+h*.05, y-h*.5, x-h*.25, y-h*.78, h*.04, h*.03), c))
    out.append(poly(quad_seg(x+h*.05, y-h*.5, x+h*.3, y-h*.8, h*.04, h*.03), c))
    out.append(poly(circle_pts(x+h*.03, y-h*.85, h*.6, h*.1, 12, r, .1), c))
    return out

def p_rock(r, x, y, h, c, **k):
    n = r.randint(5, 7); w = h*r.uniform(1.0, 1.8)
    pts = [(x-w/2, y+2)]
    for i in range(1, n):
        t = i/n; pts.append((x-w/2 + w*t, y - h*math.sin(math.pi*t)*r.uniform(.6, 1.1)))
    pts.append((x+w/2, y+2)); return [poly(pts, c)]

def p_iceberg(r, x, y, h, c, **k):
    w = h*r.uniform(1.2, 2.2); pts = [(x-w/2, y)]
    for i in range(1, 6):
        t = i/6; pts.append((x-w/2 + w*t + r.uniform(-.05, .05)*w, y - h*r.uniform(.55, 1.0)))
    pts.append((x+w/2, y)); return [poly(pts, c)]

def p_crystal(r, x, y, h, c, glow=None, **k):
    out = []
    for i in range(r.randint(3, 6)):
        ang = math.pi/2 + r.uniform(-.55, .55); L = h*r.uniform(.4, 1.0); w = L*r.uniform(.14, .22)
        bx = x + r.uniform(-.2, .2)*h
        ex, ey = bx + math.cos(ang)*L, y - math.sin(ang)*L
        nx, ny = -math.sin(ang), -math.cos(ang)
        tipx, tipy = ex + math.cos(ang)*w*1.2, ey - math.sin(ang)*w*1.2
        out.append(poly([(bx+nx*w/2, y+ny*w/2), (ex+nx*w/2, ey+ny*w/2), (tipx, tipy),
                         (ex-nx*w/2, ey-ny*w/2), (bx-nx*w/2, y-ny*w/2)], c, glow, 'emit'))
    return out

def p_mushroom(r, x, y, h, c, cap=None, glow=None, **k):
    lean = r.uniform(-.12, .12)*h
    out = [poly(quad_seg(x, y, x+lean, y-h*.8, h*.12, h*.08), c)]
    cw = h*r.uniform(.45, .7)
    cap_pts = circle_pts(x+lean, y-h*.8, cw, cw*.55, 12, r, .05, math.pi, 2*math.pi)
    cap_pts += [(x+lean+cw*.9, y-h*.78), (x+lean-cw*.9, y-h*.78)]
    out.append(poly(cap_pts, cap or c, glow, 'emit'))
    return out

def p_coral(r, x, y, h, c, glow=None, **k):
    out = []
    def br(x0, y0, ang, L, w, d):
        x1 = x0 + math.cos(ang)*L; y1 = y0 - math.sin(ang)*L
        out.append(poly(quad_seg(x0, y0, x1, y1, w, w*.8), c, glow, 'emit'))
        if d > 0:
            for s in (-1, 1):
                br(x1, y1, ang + s*r.uniform(.3, .6), L*.7, w*.8, d-1)
        else:
            out.append(poly(circle_pts(x1, y1, w*.9, w*.9, 6), c, glow, 'emit'))
    br(x, y, math.pi/2, h*.35, h*.08, 3)
    return out

def p_ruin(r, x, y, h, c, vine=None, **k):
    out = []; w = h*.16
    if r.random() < .5:
        span = h*r.uniform(.6, .9); th = h
        for sx in (x-span/2, x+span/2):
            out.append(poly([(sx-w/2, y), (sx-w/2, y-th), (sx+w/2, y-th), (sx+w/2, y)], c))
        arch = [(x-span/2-w/2, y-th)]
        arch += circle_pts(x, y-th, span/2+w/2, span*.45, 10, None, 0, math.pi, 2*math.pi)
        arch += [(x+span/2+w/2, y-th), (x+span/2-w/2, y-th)]
        arch += list(reversed(circle_pts(x, y-th, span/2-w/2, span*.35, 10, None, 0, math.pi, 2*math.pi)))
        arch += [(x-span/2+w/2, y-th)]
        if r.random() < .5:  # broken arch: drop right part
            arch = arch[:len(arch)//2 - 3] + [(x+r.uniform(-.1, .1)*span, y-th-span*.3)]
        out.append(poly(arch, c))
    else:
        th = h*r.uniform(.5, 1.0)
        out.append(poly([(x-w/2, y), (x-w/2, y-th), (x-w*.1, y-th-w*.5), (x+w/2, y-th+w*.3), (x+w/2, y)], c))
    if vine:
        for i in range(2):
            vx = x + r.uniform(-.3, .3)*h
            out.append(poly(quad_seg(vx, y-h*r.uniform(.5, 1), vx+r.uniform(-6, 6), y-h*r.uniform(0, .3), 4, 3), vine))
    return out

def p_flower(r, x, y, h, c, petal=None, glow=None, **k):
    out = []; lean = r.uniform(-.2, .2)*h
    pts = [(x, y), (x+lean*.3, y-h*.5), (x+lean, y-h)]
    for a, b in zip(pts, pts[1:]):
        out.append(poly(quad_seg(a[0], a[1], b[0], b[1], h*.035, h*.025), c))
    out.append(poly([(x, y-h*.35), (x+h*.22, y-h*.5), (x+h*.05, y-h*.42)], c))
    fx, fy = x+lean, y-h; pr = h*r.uniform(.15, .22); n = r.choice([5, 6, 8])
    for i in range(n):
        a = 2*math.pi*i/n + r.random()*.2
        px, py = fx + math.cos(a)*pr, fy + math.sin(a)*pr*.6
        out.append(poly(circle_pts(px, py, pr*.75, pr*.38, 8, None, 0, a, a+2*math.pi), petal, glow, 'emit'))
    out.append(poly(circle_pts(fx, fy, pr*.35, pr*.3, 7), lighten(petal, .3), glow, 'emit'))
    return out

def p_grass(r, x, y, h, c, **k):
    out = []
    for i in range(r.randint(4, 8)):
        bx = x + r.uniform(-.5, .5)*h*.6; L = h*r.uniform(.5, 1.0); a = math.pi/2 + r.uniform(-.4, .4)
        out.append(poly([(bx-2.5, y), (bx+math.cos(a)*L, y-math.sin(a)*L), (bx+2.5, y)], c))
    return out

def p_cactus(r, x, y, h, c, **k):
    w = h*.13
    out = [poly([(x-w/2, y), (x-w/2, y-h*.9), (x, y-h), (x+w/2, y-h*.9), (x+w/2, y)], c)]
    for s in (-1, 1):
        if r.random() < .8:
            ay = y - h*r.uniform(.35, .6); ax = x + s*h*.28
            out.append(poly([(x, ay), (ax, ay), (ax, ay-h*.3), (ax+s*w*.5, ay-h*.35), (ax+s*w, ay-h*.3), (ax+s*w, ay+w), (x, ay+w)], c))
    return out

def p_spike(r, x, y, h, c, **k):
    w = h*r.uniform(.18, .32)
    return [poly([(x-w/2, y+4), (x+r.uniform(-.1, .1)*w, y-h), (x+w/2, y+4)], c)]

PROPS = dict(spike=p_spike, pine=p_pine, dead=p_dead, broad=p_broad, palm=p_palm, jungle=p_jungle, acacia=p_acacia,
             rock=p_rock, iceberg=p_iceberg, crystal=p_crystal, mushroom=p_mushroom, coral=p_coral,
             ruin=p_ruin, flower=p_flower, grass=p_grass, cactus=p_cactus)

# ---------- terrain edges ----------
def edge(kind, r, W, H, base, amp, n_pts, **k):
    if kind == 'alps':  # explicit peaks with straight, slightly broken slopes
        out = []; x = -80
        while x < W + 80:
            span = W*r.uniform(.08, .2)*k.get('width', 1); ph = amp*r.uniform(.45, 1.0)
            vy = base - amp*r.uniform(0, .25)
            out.append((x, vy))
            for t in (.3, .55):
                out.append((x + span*t*.5, vy - (vy - (base - ph))*t*2*r.uniform(.8, 1.1) if t < .5 else vy - (vy - (base-ph))*r.uniform(.85, .95)))
            out.append((x + span*.5 + r.uniform(-.05, .05)*span, base - ph))
            out.append((x + span*.72, base - ph*r.uniform(.7, .85)))
            x += span
        out.append((x, base - amp*.1))
        return out
    nz = Noise1D(r); xs = [-60 + (W+120)*i/(n_pts-1) for i in range(n_pts)]
    out = []
    for i, x in enumerate(xs):
        t = x/W
        if kind == 'hills':   y = base - amp*nz.fbm(t*k.get('freq', 3), 3)
        elif kind == 'dunes': y = base - amp*(.5+.5*math.sin(t*k.get('freq', 5)*math.pi + nz.at(t*3)*2))**1.6
        elif kind == 'peaks':
            y = base - amp*(nz.fbm(t*k.get('freq', 4), 5))**1.3 - (amp*.25*r.random() if i % 2 else 0)
        elif kind == 'flat':  y = base - amp*nz.fbm(t*2, 2)*.3
        elif kind == 'mesa':
            v = nz.at(t*k.get('freq', 3)); y = base - (amp if v > .55 else amp*.15*v)
        elif kind == 'volcano':
            cx = k.get('cx', .55); d = abs(t-cx)
            y = base - amp*max(0, 1 - d*k.get('slope', 3.2)) - amp*.08*nz.fbm(t*6)
            if d < .035: y = base - amp*(1-.035*k.get('slope', 3.2)) + amp*.05
        elif kind == 'cliff':
            side = k.get('side', 1); s = t if side > 0 else 1-t
            y = base - amp*(1/(1+math.exp(-(s-k.get('at', .62))*22))) - amp*.12*nz.fbm(t*9, 4) - (amp*.04*r.random() if i % 2 else 0)
        elif kind == 'ceiling':
            y = base + amp*nz.fbm(t*k.get('freq', 4), 4) + (amp*r.uniform(.3, 1.1) if i % 3 == 0 else 0)
        else: y = base
        out.append((x, y))
    return out

def ground_at(edge_pts, x):
    for a, b in zip(edge_pts, edge_pts[1:]):
        if a[0] <= x <= b[0]:
            t = (x-a[0])/((b[0]-a[0]) or 1); return a[1] + (b[1]-a[1])*t
    return edge_pts[-1][1]

# ---------- scene building ----------
def build(spec, W, H, mode):
    r = random.Random(spec['seed'] + (0 if mode == 'battle' else 7))
    S = min(W, H)/1080  # scale props with the short side
    portrait = H > W
    layers = []
    specs = list(spec['layers'])
    first = specs[0]
    if spec.get('far', True) and first.get('edge', 'hills') not in ('ceiling', 'none') and spec.get('special') is None:
        far = dict(base=first['base'] - .05, amp=first.get('amp', .1)*1.3 + .04, edge=first.get('edge') if first.get('edge') in ('peaks', 'alps') else 'hills', smooth=first.get('smooth', True),
                   color=mix(first['color'], spec['sky'][-1], .55), fog=.12, ek=dict(freq=2.2), rimop=.15)
        specs.insert(0, far)
    for li, L in enumerate(specs):
        lr = random.Random(spec['seed']*31 + li)
        base = L['base']*H; amp = L.get('amp', .1)*H
        npts = L.get('pts', 40 if mode == 'battle' else 18)
        kind = L.get('edge', 'hills')
        shapes = []
        if kind == 'ceiling':
            e = edge(kind, lr, W, H, base, amp, npts, **L.get('ek', {}))
            pts = [(-60, -10)] + e + [(W+60, -10)]
            shapes.append(poly(pts, L['color']))
        elif kind != 'none':
            e = edge(kind, lr, W, H, base, amp, npts, **L.get('ek', {}))
            pts = e + [(W+60, H+10), (-60, H+10)]
            shapes.append(poly(pts, L['color'], kind=L.get('kind', 'solid')))
        else:
            e = [(-60, base), (W+60, base)]
        for pr in L.get('props', []):
            fn = PROPS[pr['type']]; dens = pr.get('every', .1)*W
            x = -dens*lr.random()
            while x < W + dens:
                if lr.random() < pr.get('p', 1):
                    gy = ground_at(e, x) + pr.get('sink', .0)*H
                    if pr.get('ceil'): gy = ground_at(e, x)
                    h = pr['h']*H*lr.uniform(.7, 1.25) if not portrait else pr['h']*H*.72*lr.uniform(.7, 1.25)
                    args = {kk: vv for kk, vv in pr.items() if kk not in ('type', 'every', 'h', 'p', 'sink', 'ceil')}
                    got = fn(lr, x, gy, h, pr.get('color', L['color']), **args)
                    for g_ in got:
                        if g_['kind'] == 'solid': g_['kind'] = 'prop'
                    if pr.get('ceil'):  # hang from the ceiling: mirror vertically around gy
                        got = [dict(s, pts=[(px, 2*gy-py) for px, py in s['pts']]) for s in got]
                    shapes += got
                x += dens*lr.uniform(.6, 1.4)
        layers.append(dict(spec=L, shapes=shapes, edge=e, base=base, amp=amp))
    return layers

# ---------- SVG output ----------
def path_d(pts, smooth=False):
    if not smooth or len(pts) < 4:
        return 'M' + ' L'.join('%.1f %.1f' % p for p in pts) + ' Z'
    d = 'M%.1f %.1f' % pts[0]
    for i in range(1, len(pts)-1):
        mx = (pts[i][0]+pts[i+1][0])/2; my = (pts[i][1]+pts[i+1][1])/2
        d += ' Q%.1f %.1f %.1f %.1f' % (pts[i][0], pts[i][1], mx, my)
    d += ' L%.1f %.1f Z' % pts[-1]
    return d

def path_edge(pts):
    """Terrain polygon: edge points then two corner points; smooth only the edge."""
    e, corners = pts[:-2], pts[-2:]
    d = 'M%.1f %.1f' % e[0]
    for i in range(1, len(e)-1):
        mx = (e[i][0]+e[i+1][0])/2; my = (e[i][1]+e[i+1][1])/2
        d += ' Q%.1f %.1f %.1f %.1f' % (e[i][0], e[i][1], mx, my)
    d += ' L%.1f %.1f' % e[-1]
    return d + ''.join(' L%.1f %.1f' % c for c in corners) + ' Z'

def bbox(shapes):
    xs = [p[0] for s in shapes for p in s['pts']]; ys = [p[1] for s in shapes for p in s['pts']]
    return min(xs), min(ys), max(xs), max(ys)

def mesh_tris(r, x0, y0, x1, y1, cell):
    nx = max(2, int((x1-x0)/cell)+2); ny = max(2, int((y1-y0)/cell)+2)
    g = [[(x0 + i*cell + (r.uniform(-.38, .38)*cell if 0 < i < nx-1 else 0),
           y0 + j*cell + (r.uniform(-.38, .38)*cell if 0 < j < ny-1 else 0)) for i in range(nx)] for j in range(ny)]
    tris = []
    for j in range(ny-1):
        for i in range(nx-1):
            a, b, c, d = g[j][i], g[j][i+1], g[j+1][i], g[j+1][i+1]
            if (i+j) % 2: tris += [(a, b, d), (a, d, c)]
            else: tris += [(a, b, c), (b, d, c)]
    return tris

class SVG:
    def __init__(self, W, H):
        self.W, self.H = W, H; self.defs = []; self.body = []; self.n = 0
    def uid(self, p): self.n += 1; return '%s%d' % (p, self.n)
    def out(self):
        return ('<svg xmlns="http://www.w3.org/2000/svg" width="%d" height="%d" viewBox="0 0 %d %d">'
                % (self.W, self.H, self.W, self.H) + '<defs>' + ''.join(self.defs) + '</defs>' + ''.join(self.body) + '</svg>')

def common_defs(svg):
    svg.defs.append('<filter id="b2" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="2"/></filter>'
                    '<filter id="b6" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="6"/></filter>'
                    '<filter id="b18" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="18"/></filter>'
                    '<filter id="b50" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="50"/></filter>'
                    '<filter id="grain"><feTurbulence type="fractalNoise" baseFrequency=".85" numOctaves="2" seed="3"/>'
                    '<feColorMatrix values="0 0 0 0 .5  0 0 0 0 .5  0 0 0 0 .5  0 0 0 .55 0"/></filter>')

def sky(svg, spec, mode, r):
    W, H = svg.W, svg.H; sk = spec['sky']
    g = svg.uid('sky')
    stops = ''.join('<stop offset="%.2f" stop-color="%s"/>' % (i/(len(sk)-1), c) for i, c in enumerate(sk))
    svg.defs.append('<linearGradient id="%s" x1="0" y1="0" x2="0" y2="1">%s</linearGradient>' % (g, stops))
    svg.body.append('<rect width="%d" height="%d" fill="url(#%s)"/>' % (W, H, g))
    def skycol(y):
        t = max(0, min(1, y/H))*(len(sk)-1); i = min(int(t), len(sk)-2); return mix(sk[i], sk[i+1], t-i)
    if mode == 'card':  # faceted sky
        cell = 92
        for tri in mesh_tris(r, -10, -10, W+10, H+10, cell):
            cy = sum(p[1] for p in tri)/3
            c = shade(skycol(cy), r.uniform(.93, 1.07))
            svg.body.append('<path d="%s" fill="%s" stroke="%s" stroke-width=".6"/>' % (path_d(tri), c, c))
    # aurora
    if spec.get('aurora'):
        for k, col in enumerate(spec['aurora']):
            a = svg.uid('au')
            svg.defs.append('<linearGradient id="%s" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="%s" stop-opacity="0"/>'
                            '<stop offset=".6" stop-color="%s" stop-opacity=".55"/><stop offset="1" stop-color="%s" stop-opacity="0"/></linearGradient>' % (a, col, col, col))
            pts = []; nz = Noise1D(r)
            for i in range(30):
                x = -50 + (W+100)*i/29; y = H*(.12 + .1*k) + H*.08*math.sin(i*.45 + k) + H*.05*nz.at(i*.3)
                pts.append((x, y))
            top = [(x, y - H*.16) for x, y in pts]; bot = [(x, y + H*.05) for x, y in reversed(pts)]
            filt = 'filter="url(#b18)"' if mode == 'battle' else ''
            svg.body.append('<path d="%s" fill="url(#%s)" opacity=".8" %s/>' % (path_d(top+bot, mode == 'battle'), a, filt))
    # stars
    n = int(spec.get('stars', 120)*(W*H)/(1920*1080))
    for _ in range(n):
        x, y = r.random()*W, r.random()**1.6*H*spec.get('star_h', .55)
        s = r.random()**3*2.4 + .5
        if mode == 'card':
            svg.body.append('<path d="M%.1f %.1f l%.1f %.1f l%.1f %.1f l%.1f %.1f Z" fill="#fff4dc" opacity="%.2f"/>'
                            % (x, y-s*1.5, s, s*1.5, -s, s*1.5, -s, -s*1.5, r.uniform(.3, .85)))
        else:
            svg.body.append('<circle cx="%.1f" cy="%.1f" r="%.2f" fill="#fff4dc" opacity="%.2f"/>' % (x, y, s*.7, r.uniform(.25, .9)))
            if s > 2.2:
                svg.body.append('<circle cx="%.1f" cy="%.1f" r="%.1f" fill="#fff4dc" opacity=".25" filter="url(#b6)"/>' % (x, y, s*3))
    # moon
    m = spec.get('moon')
    if m:
        mx, my, mr = m[0]*W, m[1]*H, m[2]*min(W, H); mc = m[3]
        halo = svg.uid('halo')
        svg.defs.append('<radialGradient id="%s"><stop offset="0" stop-color="%s" stop-opacity=".45"/><stop offset=".35" stop-color="%s" stop-opacity=".12"/>'
                        '<stop offset="1" stop-color="%s" stop-opacity="0"/></radialGradient>' % (halo, mc, mc, mc))
        svg.body.append('<circle cx="%.1f" cy="%.1f" r="%.1f" fill="url(#%s)"/>' % (mx, my, mr*7, halo))
        if mode == 'card':
            pts = circle_pts(mx, my, mr, mr, 14)
            svg.body.append('<path d="%s" fill="%s"/>' % (path_d(pts), mc))
            for i in range(14):
                a, b = pts[i], pts[(i+1) % 14]
                svg.body.append('<path d="%s" fill="%s"/>' % (path_d([(mx, my), a, b]), shade(mc, r.uniform(.88, 1.04))))
        else:
            mg = svg.uid('moon')
            svg.defs.append('<radialGradient id="%s" cx=".42" cy=".4"><stop offset="0" stop-color="%s"/><stop offset="1" stop-color="%s"/></radialGradient>'
                            % (mg, lighten(mc, .4), shade(mc, .82)))
            svg.body.append('<circle cx="%.1f" cy="%.1f" r="%.1f" fill="url(#%s)"/>' % (mx, my, mr, mg))
            for _ in range(6):
                a = r.random()*6.28; d = r.random()*mr*.6; cr = mr*r.uniform(.08, .2)
                svg.body.append('<circle cx="%.1f" cy="%.1f" r="%.1f" fill="%s" opacity=".18"/>' % (mx+math.cos(a)*d, my+math.sin(a)*d, cr, shade(mc, .6)))
    # clouds
    for cl in spec.get('clouds', []):
        cy = cl['y']*H; col = cl['color']
        for i in range(cl.get('n', 5)):
            cx = r.random()*W; w = W*r.uniform(.18, .38); h = H*r.uniform(.03, .06)
            blobs = []
            for j in range(5):
                bx = cx + (j-2)*w*.2; by = cy + r.uniform(-1, 1)*h*.5
                blobs.append(circle_pts(bx, by, w*.16, h*r.uniform(.8, 1.4), 10 if mode == 'card' else 24, r, .08))
            for b in blobs:
                if mode == 'card':
                    svg.body.append('<path d="%s" fill="%s" opacity="%.2f"/>' % (path_d(b), shade(col, r.uniform(.9, 1.1)), cl.get('op', .7)))
                else:
                    svg.body.append('<path d="%s" fill="%s" opacity="%.2f" filter="url(#b6)"/>' % (path_d(b, True), col, cl.get('op', .7)))
    return m

def facet_group(svg, shapes, color, r, cell, light, top_y, bot_y):
    """Draw shapes of one colour as a faceted low-poly mesh clipped to their outline."""
    cid = svg.uid('c')
    svg.defs.append('<clipPath id="%s">%s</clipPath>' % (cid, ''.join('<path d="%s"/>' % path_d(s['pts']) for s in shapes)))
    x0, y0, x1, y1 = bbox(shapes)
    x0, y0 = max(x0, -20), max(y0, -20); x1, y1 = min(x1, svg.W+20), min(y1, svg.H+20)
    if x1 <= x0 or y1 <= y0: return
    parts = ['<g clip-path="url(#%s)">' % cid, '<rect x="%.1f" y="%.1f" width="%.1f" height="%.1f" fill="%s"/>' % (x0, y0, x1-x0, y1-y0, color)]
    lx = light
    for tri in mesh_tris(r, x0-cell, y0-cell, x1+cell, y1+cell, cell):
        cx = sum(p[0] for p in tri)/3; cy = sum(p[1] for p in tri)/3
        hgt = max(1, bot_y - top_y)
        f = r.uniform(.86, 1.12) + .22*max(0, 1-(cy-top_y)/(hgt*.5)) + .1*max(0, 1-abs(cx-lx)/(svg.W*.6))
        c = shade(color, f)
        parts.append('<path d="%s" fill="%s" stroke="%s" stroke-width=".7"/>' % (path_d(tri), c, c))
    parts.append('</g>')
    svg.body.append(''.join(parts))

def render(spec, mode):
    W, H = (1024, 1536) if mode == 'card' else (1920, 1080)
    svg = SVG(W, H); common_defs(svg); r = random.Random(spec['seed']*7 + (1 if mode == 'card' else 2))
    if spec.get('special') == 'cathedral':
        cathedral(svg, spec, mode, r); finish(svg, spec, mode, r); return svg.out()
    moon = sky(svg, spec, mode, r)
    lightx = (moon[0] if moon else .5)*W
    rim = moon[3] if moon else spec.get('rim', '#c8d4ff')
    hz = spec.get('horizon_glow')
    if hz is None and moon: hz = (moon[0], spec['layers'][0]['base'] - .02, moon[3], .22)
    if hz:
        gid = svg.uid('hz')
        svg.defs.append('<radialGradient id="%s"><stop offset="0" stop-color="%s" stop-opacity="%.2f"/><stop offset="1" stop-color="%s" stop-opacity="0"/></radialGradient>'
                        % (gid, hz[2], hz[3], hz[2]))
        svg.body.append('<ellipse cx="%.0f" cy="%.0f" rx="%.0f" ry="%.0f" fill="url(#%s)"/>' % (hz[0]*W, hz[1]*H, W*.6, H*.18, gid))
    layers = build(spec, W, H, mode)
    for li, L in enumerate(layers):
        ls = L['spec']
        # water band drawn before this layer if requested
        if ls.get('water_before'):
            water(svg, spec, mode, r, ls['water_before'], moon)
        groups = {}
        for s in L['shapes']: groups.setdefault((s['color'], s['glow']), []).append(s)
        for (col, glow), shp in groups.items():
            kind = 'solid' if any(x['kind'] == 'solid' for x in shp) else 'prop'
            if glow:
                gl = ''.join('<path d="%s"/>' % path_d(s['pts']) for s in shp)
                svg.body.append('<g fill="%s" opacity="%.2f" filter="url(#%s)">%s</g>' % (glow, .45 if mode == 'battle' else .35, 'b18' if mode == 'battle' else 'b6', gl))
            if mode == 'card':
                x0, y0, x1, y1 = bbox(shp)
                facet_group(svg, shp, col, r, ls.get('cell', 60), lightx, y0, min(y1, H))
            else:
                x0, y0, x1, y1 = bbox(shp); gid = svg.uid('lg')
                y0 = min(y0, L['base'] - L['amp']); y1 = max(y1, L['base'] + H*.15)
                top = mix(col, rim, ls.get('rimmix', .07)) if not glow else lighten(col, .25)
                svg.defs.append('<linearGradient id="%s" gradientUnits="userSpaceOnUse" x1="0" y1="%.0f" x2="0" y2="%.0f">'
                                '<stop offset="0" stop-color="%s"/><stop offset="1" stop-color="%s"/></linearGradient>'
                                % (gid, y0, min(y1, H), top, shade(col, .5)))
                smooth = ls.get('smooth', True) and ls.get('edge') != 'alps'
                d = ''.join('<path d="%s"/>' % (path_edge(s['pts']) if (smooth and s['kind'] == 'solid' and len(s['pts']) > 12 and ls.get('edge', 'hills') != 'ceiling') else path_d(s['pts'])) for s in shp)
                if not glow and kind == 'solid' and ls.get('edge', 'hills') not in ('none', 'ceiling'):
                    e = [p for p in L['edge'] if -40 < p[0] < W+40]
                    if len(e) > 2:
                        ed = 'M' + ' L'.join('%.1f %.1f' % p for p in e)
                        svg.body.append('<path d="%s" fill="none" stroke="%s" stroke-width="5" opacity="%.2f" filter="url(#b6)"/>'
                                        % (ed, rim, ls.get('rimop', .35)))
                svg.body.append('<g fill="url(#%s)">%s</g>' % (gid, d))
        if ls.get('snow'):
            e = [p for p in L['edge']]
            nz = Noise1D(r); sl = L['base'] - L['amp']*ls.get('snow_line', .55)
            cap = e + [(x, max(y, min(y + L['amp']*ls.get('snow_depth', .18), sl + L['amp']*.08*(nz.at(x/120)-.5)))) for x, y in reversed(e)]
            if mode == 'card':
                facet_group(svg, [poly(cap, ls['snow'])], ls['snow'], r, ls.get('cell', 60), lightx, *bbox([poly(cap, '')])[1::2])
            else:
                svg.body.append('<path d="%s" fill="%s" opacity=".85"/>' % (path_d(cap), ls['snow']))
        # mist at this layer's foot
        fog = ls.get('fog', spec.get('fog_amt', .3))
        if fog > 0:
            fid = svg.uid('fg'); fc = spec['fog']; y0 = L['base'] - L['amp']*.6 - H*.04; y1 = L['base'] + H*.1
            svg.defs.append('<linearGradient id="%s" gradientUnits="userSpaceOnUse" x1="0" y1="%.0f" x2="0" y2="%.0f">'
                            '<stop offset="0" stop-color="%s" stop-opacity="0"/><stop offset=".6" stop-color="%s" stop-opacity="%.2f"/>'
                            '<stop offset="1" stop-color="%s" stop-opacity="0"/></linearGradient>' % (fid, y0, y1, fc, fc, fog*.38, fc))
            if True:
                svg.body.append('<rect x="0" y="%.0f" width="%d" height="%.0f" fill="url(#%s)"/>' % (y0, W, y1-y0, fid))
    if moon and mode == 'battle' and spec.get('rays', False):
        extra(svg, spec, mode, r, dict(type='rays', x=moon[0], y=moon[1], a=95 if moon[0] > .5 else 85, n=5, color=moon[3]))
    for ex in spec.get('extras', []): extra(svg, spec, mode, r, ex)
    frame(svg, spec, mode, r, layers)
    particles(svg, spec, mode, r)
    finish(svg, spec, mode, r)
    return svg.out()

def water(svg, spec, mode, r, w, moon):
    W, H = svg.W, svg.H; y = w['y']*H; col = w['color']
    gid = svg.uid('wt')
    svg.defs.append('<linearGradient id="%s" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="%s"/><stop offset="1" stop-color="%s"/></linearGradient>'
                    % (gid, lighten(col, .12), shade(col, .6)))
    if mode == 'card':
        cid = svg.uid('wc')
        svg.defs.append('<clipPath id="%s"><rect x="0" y="%.0f" width="%d" height="%.0f"/></clipPath>' % (cid, y, W, H-y))
        parts = ['<g clip-path="url(#%s)">' % cid]
        for tri in mesh_tris(r, -20, y-20, W+20, H+20, 70):
            cy = sum(p[1] for p in tri)/3; t = (cy-y)/(H-y)
            c = shade(mix(lighten(col, .12), shade(col, .6), max(0, min(1, t))), r.uniform(.9, 1.1))
            parts.append('<path d="%s" fill="%s" stroke="%s" stroke-width=".7"/>' % (path_d(tri), c, c))
        parts.append('</g>'); svg.body.append(''.join(parts))
    else:
        svg.body.append('<rect x="0" y="%.0f" width="%d" height="%.0f" fill="url(#%s)"/>' % (y, W, H-y, gid))
    if moon:  # reflection streak
        mx = moon[0]*W; mc = moon[3]
        for i in range(int(40 if mode == 'battle' else 18)):
            yy = y + (H-y)*(i/40 if mode == 'battle' else i/18)**1.3 + 4
            ww = (20 + 160*(yy-y)/(H-y))*r.uniform(.4, 1.2)
            xx = mx + r.uniform(-1, 1)*ww*.6
            if mode == 'card':
                svg.body.append('<path d="M%.0f %.0f L%.0f %.0f L%.0f %.0f Z" fill="%s" opacity="%.2f"/>' % (xx-ww/2, yy, xx+ww/2, yy, xx, yy+3, mc, r.uniform(.2, .5)))
            else:
                svg.body.append('<rect x="%.0f" y="%.0f" width="%.0f" height="2" rx="1" fill="%s" opacity="%.2f" filter="url(#b2)"/>' % (xx-ww/2, yy, ww, mc, r.uniform(.2, .55)))
    for _ in range(w.get('sparkles', 0)):
        xx, yy = r.random()*W, y + r.random()*(H-y); s = r.uniform(1, 3)
        svg.body.append('<circle cx="%.0f" cy="%.0f" r="%.1f" fill="%s" opacity="%.2f"%s/>' % (xx, yy, s, w.get('spark', '#7ff'), r.uniform(.3, .8), ' filter="url(#b2)"' if mode == 'battle' else ''))

def extra(svg, spec, mode, r, ex):
    W, H = svg.W, svg.H
    if ex['type'] == 'lightning':
        x = ex['x']*W; y = 0; pts = [(x, y)]
        while y < ex['to']*H:
            y += H*r.uniform(.03, .06); x += W*r.uniform(-.03, .03); pts.append((x, y))
        d = 'M' + ' L'.join('%.0f %.0f' % p for p in pts)
        svg.body.append('<path d="%s" stroke="#cfe0ff" stroke-width="%d" fill="none" opacity=".35" filter="url(#b18)"/>' % (d, 26 if mode == 'battle' else 18))
        svg.body.append('<path d="%s" stroke="#f4f8ff" stroke-width="%d" fill="none" stroke-linejoin="bevel"/>' % (d, 4 if mode == 'battle' else 5))
        svg.body.append('<rect width="%d" height="%d" fill="#9fb6ff" opacity=".06"/>' % (W, H))
    if ex['type'] == 'lava':
        for _ in range(ex.get('n', 5)):
            x = r.random()*W; y = H*r.uniform(ex['y0'], ex['y1']); pts = [(x, y)]
            for i in range(8):
                x += W*r.uniform(.02, .06)*(1 if r.random() < .6 else -1); y += H*r.uniform(.005, .02); pts.append((x, y))
            d = 'M' + ' L'.join('%.0f %.0f' % p for p in pts)
            svg.body.append('<path d="%s" stroke="#ff6a1a" stroke-width="%d" fill="none" opacity=".6" filter="url(#b6)"/>' % (d, 16))
            svg.body.append('<path d="%s" stroke="#ffb347" stroke-width="3" fill="none" stroke-linejoin="bevel"/>' % d)
    if ex['type'] == 'surf':
        for _ in range(60 if mode == 'battle' else 30):
            x = W*r.uniform(ex['x0'], ex['x1']); y = H*ex['y'] + r.uniform(0, H*.12)**1.2; w = r.uniform(20, 90)
            if mode == 'battle':
                svg.body.append('<rect x="%.0f" y="%.0f" width="%.0f" height="2.5" rx="1" fill="#c8d4e8" opacity="%.2f" filter="url(#b2)"/>' % (x, y, w, r.uniform(.15, .45)))
            else:
                svg.body.append('<path d="M%.0f %.0f L%.0f %.0f L%.0f %.0f Z" fill="#c8d4e8" opacity="%.2f"/>' % (x, y, x+w, y, x+w*.5, y+4, r.uniform(.15, .4)))
    if ex['type'] == 'glow':
        gid = svg.uid('gw')
        svg.defs.append('<radialGradient id="%s"><stop offset="0" stop-color="%s" stop-opacity="%.2f"/><stop offset="1" stop-color="%s" stop-opacity="0"/></radialGradient>'
                        % (gid, ex['color'], ex.get('op', .5), ex['color']))
        svg.body.append('<ellipse cx="%.0f" cy="%.0f" rx="%.0f" ry="%.0f" fill="url(#%s)"/>' % (ex['x']*W, ex['y']*H, ex['rx']*W, ex['ry']*H, gid))
    if ex['type'] == 'rays' and mode == 'battle':
        mx, my = ex['x']*W, ex['y']*H
        for i in range(ex.get('n', 5)):
            a = math.radians(ex.get('a', 100) + r.uniform(-18, 18)); L = H*1.4; w = r.uniform(30, 90)
            ex2, ey2 = mx + math.cos(a)*L, my + math.sin(a)*L
            svg.body.append('<path d="M%.0f %.0f L%.0f %.0f L%.0f %.0f Z" fill="%s" opacity="%.2f" filter="url(#b18)"/>'
                            % (mx, my, ex2-w, ey2, ex2+w, ey2, ex['color'], r.uniform(.04, .09)))

def frame(svg, spec, mode, r, layers):
    """Near-black silhouettes at the screen edges: depth, and they keep the centre calm."""
    fr = spec.get('frame')
    if not fr: return
    W, H = svg.W, svg.H
    col = spec.get('frame_color', shade(spec['layers'][-1]['color'], .55))
    shapes = []
    for f in fr:
        fn = PROPS[f['type']]; x = (-.02 if f['side'] < 0 else 1.02)*W
        if mode == 'card': x = (.0 if f['side'] < 0 else 1.0)*W
        h = f['h']*H*(.8 if mode == 'card' else 1)
        args = {k: v for k, v in f.items() if k not in ('type', 'side', 'h', 'top')}
        got = fn(r, x, H*1.02, h, col, **args)
        if f.get('top'):  # hang from the top edge
            got = [dict(g, pts=[(px, H*1.02 - py) for px, py in g['pts']]) for g in got]
        shapes += got
    if not shapes: return
    if mode == 'card':
        facet_group(svg, shapes, col, r, 50, W*.5, *bbox(shapes)[1::2])
    else:
        svg.body.append('<g fill="%s">%s</g>' % (col, ''.join('<path d="%s"/>' % path_d(s['pts']) for s in shapes)))

def particles(svg, spec, mode, r):
    W, H = svg.W, svg.H
    for pk in spec.get('particles', []):
        n = int(pk['n']*(1 if mode == 'battle' else .6))
        for _ in range(n):
            x = r.random()*W; y = H*r.uniform(pk.get('y0', .2), pk.get('y1', 1))
            s = r.uniform(*pk.get('size', (1.5, 4)))*(1.3 if mode == 'card' else 1)
            col = r.choice(pk['color']) if isinstance(pk['color'], list) else pk['color']
            op = r.uniform(.35, .9)
            k = pk['kind']
            if k in ('glow',):
                if mode == 'battle':
                    svg.body.append('<circle cx="%.0f" cy="%.0f" r="%.1f" fill="%s" opacity="%.2f" filter="url(#b6)"/>' % (x, y, s*4, col, op*.5))
                    svg.body.append('<circle cx="%.0f" cy="%.0f" r="%.1f" fill="%s" opacity="%.2f"/>' % (x, y, s*.8, lighten(col, .5), op))
                else:
                    svg.body.append('<circle cx="%.0f" cy="%.0f" r="%.1f" fill="%s" opacity="%.2f" filter="url(#b6)"/>' % (x, y, s*3, col, op*.35))
                    svg.body.append('<path d="M%.0f %.0f l%.1f %.1f l%.1f %.1f l%.1f %.1f Z" fill="%s" opacity="%.2f"/>' % (x, y-s, s, s, -s, s, -s, -s, lighten(col, .4), op))
            elif k == 'streak':  # rain / snow-drift
                L = s*pk.get('len', 8); a = pk.get('angle', 1.75)
                svg.body.append('<path d="M%.0f %.0f l%.1f %.1f" stroke="%s" stroke-width="%.1f" opacity="%.2f" stroke-linecap="round"/>'
                                % (x, y, math.cos(a)*L, math.sin(a)*L, col, s*.35, op*.5))
            elif k == 'flake':
                if mode == 'battle':
                    svg.body.append('<circle cx="%.0f" cy="%.0f" r="%.1f" fill="%s" opacity="%.2f"%s/>' % (x, y, s*.6, col, op*.8, ' filter="url(#b2)"' if s > 3 else ''))
                else:
                    svg.body.append('<path d="M%.0f %.0f l%.1f %.1f l%.1f %.1f l%.1f %.1f Z" fill="%s" opacity="%.2f"/>' % (x, y-s*.7, s*.7, s*.7, -s*.7, s*.7, -s*.7, -s*.7, col, op*.8))
            elif k == 'leaf':
                a = r.random()*6.28; pts = circle_pts(x, y, s*1.6, s*.7, 6, None, 0, a, a+6.28)
                svg.body.append('<path d="%s" fill="%s" opacity="%.2f"/>' % (path_d(pts), col, op))

def finish(svg, spec, mode, r):
    W, H = svg.W, svg.H
    vg = svg.uid('vg')
    svg.defs.append('<radialGradient id="%s" cx=".5" cy="%.2f" r="%.2f"><stop offset=".45" stop-color="#07050c" stop-opacity="0"/>'
                    '<stop offset="1" stop-color="#07050c" stop-opacity="%.2f"/></radialGradient>' % (vg, .45 if mode == 'battle' else .5, .85 if mode == 'battle' else .8, .75))
    svg.body.append('<rect width="%d" height="%d" fill="url(#%s)"/>' % (W, H, vg))
    dim = spec.get('dim', .18) + (.12 if mode == 'card' else 0)
    svg.body.append('<rect width="%d" height="%d" fill="#0b0814" opacity="%.2f"/>' % (W, H, dim))
    if mode == 'battle':
        svg.body.append('<rect width="%d" height="%d" filter="url(#grain)" opacity=".07"/>' % (W, H))

# ---------- the cathedral interior ----------
def cathedral(svg, spec, mode, r):
    """A gothic nave in one-point perspective: arcade, ribbed vault, rose window, candles."""
    W, H = svg.W, svg.H
    portrait = H > W
    vx, vy = W*.5, H*(.6 if portrait else .62)
    f = (330 if portrait else 640)*(W/1024 if portrait else W/1920)
    eye = 1.7
    def P(X, Y, Z): return (vx + X*f/Z, vy - (Y-eye)*f/Z)
    lead = '#0d0912'
    def draw(pts, col, glow=None, op=1, cell=46):
        if glow:
            svg.body.append('<path d="%s" fill="%s" opacity=".35" filter="url(#%s)"/>' % (path_d(pts), glow, 'b18' if mode == 'battle' else 'b6'))
        if mode == 'card' and not glow and op == 1:
            facet_group(svg, [poly(pts, col)], col, r, cell, vx, min(p[1] for p in pts), max(p[1] for p in pts))
        else:
            svg.body.append('<path d="%s" fill="%s" opacity="%.2f"/>' % (path_d(pts), col, op))
    def line(pts, col, w, op=1):
        svg.body.append('<path d="M%s" fill="none" stroke="%s" stroke-width="%.1f" opacity="%.2f" stroke-linejoin="round"/>'
                        % (' L'.join('%.1f %.1f' % p for p in pts), col, w, op))
    HW, CH, AP, ZB, ZN = 6.0, 12.0, 19.0, 32.0, 1.2   # half width, column height, vault apex, back wall, near plane
    svg.body.append('<rect width="%d" height="%d" fill="#0c0912"/>' % (W, H))
    # vault: ceiling panels between consecutive rib arches
    zs = [ZN + 3.2*i for i in range(14) if ZN + 3.2*i < ZB] + [ZB]
    def arch(z, n=10):
        pts = []
        for i in range(n+1):
            t = i/n; a = math.pi/2*t
            pts.append((HW*(1-math.sin(a)*.98), CH + (AP-CH)*math.sin(a)**.9, z))
        return pts
    for z0, z1 in zip(zs, zs[1:]):
        for side in (-1, 1):
            a0 = [(side*x, y, z0) for x, y, _ in arch(z0)]; a1 = [(side*x, y, z1) for x, y, _ in arch(z1)]
            for (p0, p1), (q0, q1) in zip(zip(a0, a0[1:]), zip(a1, a1[1:])):
                col = shade('#1e1828', .8 + .25*((p0[1]-CH)/(AP-CH)))
                draw([P(*p0), P(*p1), P(*q1), P(*q0)], col, cell=60)
    # side walls (clerestory band above the arcade) and the aisles behind the arcade
    for side in (-1, 1):
        draw([P(side*HW, 0, ZN), P(side*HW, CH, ZN), P(side*HW, CH, ZB), P(side*HW, 0, ZB)], '#171221')
        draw([P(side*(HW+4), 0, ZN), P(side*(HW+4), CH*.62, ZN), P(side*(HW+4), CH*.62, ZB), P(side*(HW+4), 0, ZB)], '#0f0b16')
    # back wall with gable
    draw([P(-HW, 0, ZB), P(-HW, CH, ZB), P(0, AP, ZB), P(HW, CH, ZB), P(HW, 0, ZB)], '#1f1929')
    # rose window
    cx, cy = P(0, 13.4, ZB); R = 4.4*f/ZB
    svg.body.append('<circle cx="%.1f" cy="%.1f" r="%.1f" fill="#7a5ac8" opacity=".45" filter="url(#%s)"/>' % (cx, cy, R*1.5, 'b50' if mode == 'battle' else 'b18'))
    jewels = ['#3b2a78', '#6c2440', '#23506c', '#2c3f86', '#7a5a22', '#4a2a6a']
    svg.body.append('<circle cx="%.1f" cy="%.1f" r="%.1f" fill="%s"/>' % (cx, cy, R*1.08, lead))
    for i in range(16):
        a0 = 2*math.pi*i/16; a1 = 2*math.pi*(i+1)/16; am = (a0+a1)/2
        pts = [(cx+math.cos(a0)*R*.55, cy+math.sin(a0)*R*.55), (cx+math.cos(a0)*R*.97, cy+math.sin(a0)*R*.97),
               (cx+math.cos(am)*R*1.0, cy+math.sin(am)*R*1.0), (cx+math.cos(a1)*R*.97, cy+math.sin(a1)*R*.97),
               (cx+math.cos(a1)*R*.55, cy+math.sin(a1)*R*.55)]
        draw(pts, shade(jewels[i % len(jewels)], r.uniform(.9, 1.15)))
        line(pts + [pts[0]], lead, max(1.5, R*.03))
    for i in range(8):
        a0 = 2*math.pi*i/8; a1 = 2*math.pi*(i+1)/8; am = (a0+a1)/2
        pts = [(cx+math.cos(a0)*R*.2, cy+math.sin(a0)*R*.2), (cx+math.cos(am - .25)*R*.5, cy+math.sin(am - .25)*R*.5),
               (cx+math.cos(am)*R*.54, cy+math.sin(am)*R*.54), (cx+math.cos(am + .25)*R*.5, cy+math.sin(am + .25)*R*.5),
               (cx+math.cos(a1)*R*.2, cy+math.sin(a1)*R*.2)]
        draw(pts, shade(jewels[(i*3+1) % len(jewels)], 1.1))
        line(pts + [pts[0]], lead, max(1.2, R*.025))
    draw(circle_pts(cx, cy, R*.2, R*.2, 12), '#a8863a', '#d9b45a')
    line(circle_pts(cx, cy, R*1.04, R*1.04, 40) + [circle_pts(cx, cy, R*1.04, R*1.04, 40)[0]], '#2a2234', max(2, R*.06))
    # three lancets under the rose
    for k, lx in enumerate((-2.6, 0, 2.6)):
        top = 9.6 if lx == 0 else 8.6
        wx = .75 if lx == 0 else .62
        pts = [P(lx-wx, 3.4, ZB), P(lx-wx, top, ZB), P(lx, top+1.2, ZB), P(lx+wx, top, ZB), P(lx+wx, 3.4, ZB)]
        svg.body.append('<path d="%s" fill="#3a5aaa" opacity=".3" filter="url(#b18)"/>' % path_d(pts))
        svg.body.append('<path d="%s" fill="%s"/>' % (path_d(pts), lead))
        inner = [P(lx-wx*.85, 3.6, ZB), P(lx-wx*.85, top, ZB), P(lx, top+1.0, ZB), P(lx+wx*.85, top, ZB), P(lx+wx*.85, 3.6, ZB)]
        draw(inner, ['#2c3f86', '#3b2a78', '#23506c'][k])
        for j in range(1, 6):  # glazing bars
            yy = 3.6 + (top-3.6)*j/6; line([P(lx-wx*.85, yy, ZB), P(lx+wx*.85, yy, ZB)], lead, 2)
        line([P(lx, 3.6, ZB), P(lx, top+1.0, ZB)], lead, 2)
    # floor, carpet runner, altar
    draw([P(-HW-4, 0, ZN), P(-HW-4, 0, ZB), P(HW+4, 0, ZB), P(HW+4, 0, ZN)], '#191420', cell=60)
    for i in range(-5, 6):
        line([P(i*1.2, 0, ZN), P(i*1.2, 0, ZB)], '#241d2e', 1.5, .8)
    for z in zs:
        line([P(-HW-4, 0, z), P(HW+4, 0, z)], '#241d2e', 1.5, .8)
    draw([P(-1.1, 0, ZN), P(-1.1, 0, ZB-4), P(1.1, 0, ZB-4), P(1.1, 0, ZN)], '#2a0d16')
    draw([P(-1.8, 0, ZB-4), P(-1.8, 1.3, ZB-4), P(1.8, 1.3, ZB-4), P(1.8, 0, ZB-4)], '#2a2030')
    draw([P(-1.9, 1.3, ZB-4), P(-1.9, 1.45, ZB-4), P(1.9, 1.45, ZB-4), P(1.9, 1.3, ZB-4)], '#8a6a3a')
    # light shafts from the clerestory onto the floor
    for side in (-1, 1):
        for i in range(2, len(zs)-3, 2):
            z = (zs[i]+zs[i+1])/2
            top_a, top_b = P(side*HW, CH-1, z-1), P(side*HW, CH-1, z+1)
            fx = -side*1.5
            bot_a, bot_b = P(fx, 0, z+5), P(fx, 0, z+9)
            col = ['#8a7aff', '#ff8aa8', '#7ad0ff'][i % 3]
            svg.body.append('<path d="%s" fill="%s" opacity=".07" %s/>' % (path_d([top_a, top_b, bot_b, bot_a]), col, 'filter="url(#b18)"' if mode == 'battle' else ''))
    # arcade: piers and pointed arches, far to near
    for side in (-1, 1):
        for i in range(len(zs)-2, -1, -1):
            z = zs[i]; zn = zs[i+1] if i+1 < len(zs) else z
            if i+1 < len(zs):
                # pointed arch between this pier and the next
                za, zb = z + .45, zn - .45; zm = (za+zb)/2
                pts = []
                for t in range(9):
                    u = t/8; zz = za + (zb-za)*u
                    yy = 7.6 + 2.6*math.sin(math.pi*u)**.6
                    pts.append(P(side*HW*.95, yy, zz))
                outer = [P(side*HW*.95, CH-.2, za)] + pts + [P(side*HW*.95, CH-.2, zb)]
                draw(outer, '#221b2d')
                line(pts, '#2e2639', max(1.5, 3*f/z/40), 1)
            # pier: front and inner faces
            pw = .55
            a, b2 = P(side*(HW*.95 - pw), 0, z), P(side*(HW*.95 - pw), CH, z)
            c, d = P(side*(HW*.95 + pw), CH, z), P(side*(HW*.95 + pw), 0, z)
            draw([a, b2, c, d], shade('#2a2234', 1.0 + .02*i))
            ia, ib = P(side*(HW*.95 - pw), 0, z), P(side*(HW*.95 - pw), CH, z)
            ic, idd = P(side*(HW*.95 - pw), CH, z+.9), P(side*(HW*.95 - pw), 0, z+.9)
            draw([ia, ib, ic, idd], '#1c1626')
            cap0, cap1 = P(side*(HW*.95 - pw*1.4), CH*.62, z), P(side*(HW*.95 + pw*1.4), CH*.62+.4, z)
            draw([cap0, (cap1[0], cap0[1]), cap1, (cap0[0], cap1[1])], '#3a3046')
            # candle stand at the pier foot
            if 0 < i < len(zs)-2:
                bx = side*(HW*.95 - pw - .7)
                draw([P(bx-.05, 0, z), P(bx-.05, 1.5, z), P(bx+.05, 1.5, z), P(bx+.05, 0, z)], '#4a3a2a')
                fx_, fy_ = P(bx, 1.75, z); fr = max(2, .14*f/z)
                svg.body.append('<circle cx="%.1f" cy="%.1f" r="%.1f" fill="#ffb040" opacity=".22" filter="url(#%s)"/>' % (fx_, fy_, fr*3.2, 'b18' if mode == 'battle' else 'b6'))
                draw(circle_pts(fx_, fy_, fr*.35, fr*.8, 7), '#ffd480')
    # vault ribs over everything far, drawn as lines at each arch
    for z in zs[:-1]:
        for side in (-1, 1):
            pts = [P(side*x, y, z) for x, y, _ in arch(z, 14)]
            line(pts, '#2e2639', max(1.5, 4*f/z/40), .9)
    particles(svg, spec, mode, r)

# ---------- biome scene descriptions ----------
def B(**k): return k
BIOMES = [
 B(id='pine_forest', frame=[dict(type='pine', side=-1, h=1.1), dict(type='dead', side=1, h=.9)],  name='Pine and dry forest', seed=11, sky=['#0d1022', '#1d2240', '#4a3f52'], moon=(.72, .2, .05, '#f3e3c0'), stars=160,
   fog='#5a5068', layers=[
    dict(base=.55, amp=.12, color='#2e2c44', edge='hills', props=[dict(type='pine', every=.018, h=.09)], fog=.35),
    dict(base=.66, amp=.08, color='#211f30', edge='hills', props=[dict(type='pine', every=.03, h=.17), dict(type='dead', every=.12, h=.16, p=.6)], fog=.25),
    dict(base=.82, amp=.05, color='#16131f', edge='hills', props=[dict(type='pine', every=.12, h=.42, p=.8), dict(type='dead', every=.25, h=.3, p=.7), dict(type='grass', every=.03, h=.05, color='#3a3324')], fog=0)],
   particles=[dict(kind='glow', n=25, color='#ffd27a', y0=.5, y1=.95, size=(1, 2.5))]),
 B(id='jungle', frame=[dict(type='palm', side=-1, h=1.0), dict(type='jungle', side=1, h=.9)],  name='Jungle', seed=12, sky=['#06141a', '#0d2a2c', '#2b4a3a'], moon=(.3, .16, .04, '#e6f3d0'), stars=90,
   fog='#2f5a48', fog_amt=.4, layers=[
    dict(base=.5, amp=.1, color='#1d3b36', edge='hills', props=[dict(type='jungle', every=.03, h=.16)]),
    dict(base=.62, amp=.08, color='#132a26', edge='hills', props=[dict(type='palm', every=.06, h=.25), dict(type='jungle', every=.05, h=.22)]),
    dict(base=.8, amp=.06, color='#0b1b18', edge='hills', props=[dict(type='palm', every=.22, h=.5, p=.7), dict(type='jungle', every=.2, h=.4, p=.7), dict(type='grass', every=.025, h=.07)], fog=0)],
   particles=[dict(kind='glow', n=60, color=['#b8ff7a', '#7affd2'], y0=.35, y1=.95, size=(1, 2.2))]),
 B(id='wet_forest', frame=[dict(type='broad', side=-1, h=1.0), dict(type='pine', side=1, h=1.15)],  name='Wet and mixed forest', seed=13, sky=['#0b1220', '#18263a', '#3a4c5a'], moon=(.62, .14, .035, '#dfe8f0'), stars=60,
   fog='#5d7383', fog_amt=.45, clouds=[dict(y=.18, color='#2a3848', n=6, op=.6)], layers=[
    dict(base=.52, amp=.1, color='#26364a', edge='hills', props=[dict(type='pine', every=.02, h=.1), dict(type='broad', every=.04, h=.08)]),
    dict(base=.64, amp=.08, color='#1a2836', edge='hills', props=[dict(type='broad', every=.05, h=.2), dict(type='pine', every=.05, h=.22)]),
    dict(base=.8, amp=.05, color='#101a22', edge='flat', props=[dict(type='broad', every=.25, h=.45, p=.8), dict(type='pine', every=.28, h=.5, p=.7), dict(type='grass', every=.03, h=.05)], fog=0, water_before=dict(y=.86, color='#1c2c3a'))],
   particles=[dict(kind='streak', n=220, color='#9fb4c8', y0=0, y1=1, size=(1, 2), len=14, angle=1.8)]),
 B(id='desert', frame=[dict(type='rock', side=-1, h=.18), dict(type='cactus', side=1, h=.45)],  name='Desert', seed=14, sky=['#0c0b1e', '#231a3a', '#5a3a4a'], moon=(.66, .22, .07, '#f6e2b8'), stars=260, star_h=.7,
   fog='#6a4a52', fog_amt=.15, layers=[
    dict(base=.6, amp=.16, color='#3a2a3a', edge='mesa', ek=dict(freq=3)),
    dict(base=.7, amp=.07, color='#3a262c', edge='dunes', ek=dict(freq=3), rimop=.55),
    dict(base=.82, amp=.09, color='#21151a', edge='dunes', ek=dict(freq=2), rimop=.6, props=[dict(type='cactus', every=.3, h=.2, p=.6), dict(type='rock', every=.35, h=.05, p=.5)], fog=0)],
   particles=[dict(kind='glow', n=10, color='#ffd9a0', y0=.6, y1=.9, size=(.8, 1.6))]),
 B(id='mountain', frame=[dict(type='rock', side=-1, h=.25), dict(type='pine', side=1, h=.8)],  name='Mountain', seed=15, sky=['#080c1c', '#16203a', '#3a4462'], moon=(.25, .15, .04, '#eef0ff'), stars=240,
   fog='#3a4460', fog_amt=.18, layers=[
    dict(base=.52, amp=.36, color='#252c44', edge='alps', ek=dict(width=1.3), smooth=False, snow='#8e9abd', snow_line=.62, snow_depth=1, fog=.2),
    dict(base=.64, amp=.26, color='#181d2e', edge='alps', smooth=False, snow='#59658a', snow_line=.7, snow_depth=1, fog=.15),
    dict(base=.8, amp=.1, color='#141a28', edge='peaks', ek=dict(freq=6), props=[dict(type='pine', every=.08, h=.12, p=.6), dict(type='rock', every=.2, h=.06)], fog=0)],
   particles=[dict(kind='flake', n=60, color='#e8eeff', y0=0, y1=1, size=(1, 2.5))]),
 B(id='cathedral', name='Giant cathedral (inside)', seed=16, special='cathedral', fog='#3a2a50', dim=.12,
   particles=[dict(kind='glow', n=70, color=['#e8d6ff', '#ffe6b0'], y0=.1, y1=.95, size=(.6, 1.6))]),
 B(id='savannah', frame=[dict(type='grass', side=-1, h=.3), dict(type='acacia', side=1, h=.7)],  name='Grassland and savannah', seed=17, sky=['#0f0c20', '#2a1e3a', '#6a4448'], moon=(.38, .5, .11, '#f7d9a6'), stars=180,
   fog='#6a4a4a', fog_amt=.25, layers=[
    dict(base=.66, amp=.03, color='#3a2836', edge='flat', props=[dict(type='acacia', every=.18, h=.12, p=.7)]),
    dict(base=.76, amp=.03, color='#251a24', edge='flat', props=[dict(type='acacia', every=.35, h=.28, p=.8), dict(type='grass', every=.02, h=.05, color='#3a2a26')]),
    dict(base=.88, amp=.02, color='#160f16', edge='flat', props=[dict(type='grass', every=.008, h=.1)], fog=0)],
   particles=[dict(kind='glow', n=20, color='#ffcf80', y0=.6, y1=.95, size=(.8, 2))]),
 B(id='arctic', frame=[dict(type='iceberg', side=-1, h=.25), dict(type='iceberg', side=1, h=.3)],  name='Arctic and iceberg', seed=18, sky=['#040a18', '#0c1a30', '#1e3048'], aurora=['#3affb0', '#8a5aff'], stars=200,
   fog='#5a7896', fog_amt=.3, layers=[
    dict(base=.6, amp=.0, color='#1a2c44', edge='none', props=[dict(type='iceberg', every=.25, h=.12, p=.8, color='#3a5878')], water_before=dict(y=.6, color='#0e1e30')),
    dict(base=.74, amp=.0, color='#2c4864', edge='none', props=[dict(type='iceberg', every=.45, h=.22, p=.8, color='#4a6a8c')]),
    dict(base=.9, amp=.04, color='#1a2a3c', edge='alps', ek=dict(width=2), smooth=False, fog=0, rimmix=.3)],
   extras=[dict(type='glow', x=.5, y=.66, rx=.5, ry=.05, color='#3affb0', op=.12)],
   particles=[dict(kind='flake', n=140, color='#eef6ff', y0=0, y1=1, size=(1, 3))]),
 B(id='swamp', frame=[dict(type='dead', side=-1, h=1.0, moss='#121c18'), dict(type='grass', side=1, h=.3)],  name='Swamp and bog', seed=19, sky=['#060c0e', '#122020', '#2a3a30'], moon=(.7, .18, .04, '#dfe8c8'), stars=60,
   fog='#4a6450', fog_amt=.5, layers=[
    dict(base=.6, amp=.05, color='#1e2e28', edge='hills', props=[dict(type='dead', every=.07, h=.14, moss='#2a3a30')]),
    dict(base=.7, amp=.02, color='#14201c', edge='flat', props=[dict(type='dead', every=.2, h=.32, moss='#24342c'), dict(type='grass', every=.04, h=.06)], water_before=dict(y=.68, color='#0e1814')),
    dict(base=.86, amp=.02, color='#0b1310', edge='flat', props=[dict(type='grass', every=.02, h=.09), dict(type='dead', every=.5, h=.6, p=.6, moss='#1c2a24')], fog=0)],
   particles=[dict(kind='glow', n=14, color=['#7affd0', '#b0ff8a'], y0=.5, y1=.85, size=(2, 4))]),
 B(id='coast', frame=[dict(type='rock', side=-1, h=.22), dict(type='rock', side=1, h=.3)],  name='Coast and tide pools', seed=20, sky=['#071024', '#122440', '#2e4a66'], moon=(.5, .2, .05, '#f0f2ff'), stars=150,
   fog='#4a6480', fog_amt=.25, layers=[
    dict(base=.6, amp=.34, color='#18243a', edge='cliff', ek=dict(side=-1, at=.72), pts=60),
    dict(base=.86, amp=.0, color='#141c28', edge='none', props=[dict(type='rock', every=.14, h=.08)], water_before=dict(y=.6, color='#0f2236', sparkles=0)),
    dict(base=.9, amp=.02, color='#10161e', edge='hills', ek=dict(freq=4), props=[dict(type='rock', every=.3, h=.1)], fog=0)],
   extras=[dict(type='surf', y=.6, x0=.3, x1=1.0), dict(type='glow', x=.25, y=.93, rx=.1, ry=.025, color='#4affe0', op=.55), dict(type='glow', x=.7, y=.95, rx=.08, ry=.02, color='#4affe0', op=.5), dict(type='glow', x=.48, y=.97, rx=.05, ry=.015, color='#4affe0', op=.45)],
   particles=[dict(kind='glow', n=20, color='#6affe8', y0=.85, y1=1, size=(1, 2))]),
 B(id='ashlands', frame=[dict(type='dead', side=-1, h=.7), dict(type='rock', side=1, h=.2)],  name='Volcanic ashlands', seed=21, sky=['#0e0608', '#2a0e10', '#5a1e14'], stars=40, rim='#ff7a3a',
   fog='#5a2a20', fog_amt=.35, clouds=[dict(y=.15, color='#2a1414', n=6, op=.8)], layers=[
    dict(base=.62, amp=.4, color='#2a1a1c', edge='volcano', ek=dict(cx=.55, slope=2.6), rimmix=.15),
    dict(base=.72, amp=.06, color='#1c1214', edge='peaks', ek=dict(freq=5)),
    dict(base=.86, amp=.04, color='#120b0c', edge='hills', props=[dict(type='rock', every=.15, h=.06), dict(type='dead', every=.4, h=.25, p=.5)], fog=0)],
   extras=[dict(type='glow', x=.55, y=.24, rx=.15, ry=.12, color='#ff5a1a', op=.55), dict(type='lava', n=5, y0=.75, y1=.9)],
   particles=[dict(kind='glow', n=80, color=['#ff8a3a', '#ffb24a'], y0=.1, y1=1, size=(.8, 2))]),
 B(id='crystal_caverns', frame=[dict(type='spike', side=-1, h=.6), dict(type='crystal', side=1, h=.5, glow='#3a4aff')],  name='Crystal caverns', seed=22, sky=['#05040c', '#0c0a1e', '#141030'], stars=0, rim='#8ab0ff',
   fog='#3a3a7a', fog_amt=.35, layers=[
    dict(base=.14, amp=.1, color='#100e1e', edge='ceiling', props=[dict(type='spike', every=.03, h=.12, ceil=True), dict(type='crystal', every=.15, h=.1, ceil=True, p=.6, color='#5a6aff', glow='#6a7aff')], fog=0),
    dict(base=.66, amp=.06, color='#16142a', edge='hills', props=[dict(type='spike', every=.06, h=.08, p=.6), dict(type='crystal', every=.1, h=.14, color='#6a5aff', glow='#7a6aff')]),
    dict(base=.82, amp=.05, color='#0c0a18', edge='hills', props=[dict(type='crystal', every=.25, h=.3, p=.8, color='#4aa0ff', glow='#5ab0ff'), dict(type='rock', every=.2, h=.06)], fog=0)],
   extras=[dict(type='glow', x=.5, y=.6, rx=.5, ry=.3, color='#4a4aff', op=.2)],
   particles=[dict(kind='glow', n=40, color=['#9ab0ff', '#c79aff'], y0=.2, y1=.95, size=(.8, 1.8))]),
 B(id='mushroom_grotto', frame=[dict(type='mushroom', side=-1, h=.7, cap='#2a1a2a', glow=None), dict(type='spike', side=1, h=.5)],  name='Mushroom grotto', seed=23, sky=['#05080a', '#0a1418', '#10202a'], stars=0, rim='#7affe0',
   fog='#2a5a5a', fog_amt=.35, layers=[
    dict(base=.12, amp=.1, color='#0c1416', edge='ceiling', props=[dict(type='spike', every=.025, h=.14, ceil=True)], fog=0),
    dict(base=.64, amp=.06, color='#122024', edge='hills', props=[dict(type='mushroom', every=.08, h=.12, cap='#3affd0', glow='#3affd0')]),
    dict(base=.84, amp=.05, color='#0a1214', edge='hills', props=[dict(type='mushroom', every=.24, h=.36, p=.85, cap='#ff6ad0', glow='#ff7ad8'), dict(type='grass', every=.04, h=.05)], fog=0)],
   extras=[dict(type='glow', x=.5, y=.7, rx=.5, ry=.25, color='#2affc0', op=.15)],
   particles=[dict(kind='glow', n=90, color=['#9affe8', '#ffaaf0'], y0=.15, y1=.95, size=(.6, 1.6))]),
 B(id='lagoon', frame=[dict(type='palm', side=-1, h=.9), dict(type='rock', side=1, h=.15)],  name='Lagoon and reef', seed=24, sky=['#04101e', '#0a2236', '#174a5a'], moon=(.78, .18, .04, '#e8fbff'), stars=170,
   fog='#2a6a7a', fog_amt=.2, layers=[
    dict(base=.58, amp=.04, color='#123040', edge='hills', ek=dict(freq=2), props=[dict(type='palm', every=.07, h=.08)]),
    dict(base=.92, amp=.0, color='#0e3a44', edge='none', props=[dict(type='coral', every=.12, h=.14, color='#ff6a8a', glow='#ff7a9a'), dict(type='coral', every=.16, h=.12, color='#ffb04a', glow='#ffc06a')], water_before=dict(y=.62, color='#0c3a4a', sparkles=60, spark='#7affff')),
    dict(base=.94, amp=.02, color='#0a1a20', edge='flat', fog=0)],
   extras=[dict(type='glow', x=.5, y=.85, rx=.6, ry=.14, color='#1affe0', op=.15)],
   particles=[dict(kind='glow', n=40, color='#7affff', y0=.65, y1=1, size=(.8, 1.8))]),
 B(id='ruins', frame=[dict(type='ruin', side=-1, h=.9), dict(type='broad', side=1, h=1.0)],  name='Overgrown ruins', seed=25, sky=['#0a0c18', '#1a1e30', '#3a3a48'], moon=(.22, .2, .05, '#efe6d0'), stars=140,
   fog='#4a5058', fog_amt=.35, layers=[
    dict(base=.56, amp=.08, color='#283040', edge='hills', props=[dict(type='broad', every=.05, h=.08), dict(type='ruin', every=.15, h=.1, p=.6)]),
    dict(base=.68, amp=.05, color='#1a2030', edge='hills', props=[dict(type='ruin', every=.2, h=.24, vine='#1e2c24'), dict(type='broad', every=.12, h=.2)]),
    dict(base=.84, amp=.04, color='#10141c', edge='hills', props=[dict(type='ruin', every=.45, h=.42, p=.8, vine='#15201a'), dict(type='grass', every=.03, h=.06)], fog=0)],
   particles=[dict(kind='glow', n=35, color='#d8ff8a', y0=.5, y1=.95, size=(1, 2.2))]),
 B(id='autumn_woods', frame=[dict(type='broad', side=-1, h=1.05, crown='#2a0e0c'), dict(type='broad', side=1, h=.95, crown='#2a120c')],  name='Autumn woods', seed=26, sky=['#0d0a18', '#241830', '#4a2a36'], moon=(.68, .16, .05, '#f8dcb0'), stars=130,
   fog='#5a3a3a', fog_amt=.3, layers=[
    dict(base=.54, amp=.1, color='#3a2430', edge='hills', props=[dict(type='broad', every=.03, h=.09, crown='#5a2a26')]),
    dict(base=.66, amp=.07, color='#2a1822', edge='hills', props=[dict(type='broad', every=.06, h=.22, crown='#6a2a1a'), dict(type='broad', every=.08, h=.2, crown='#6a4a1a')]),
    dict(base=.82, amp=.05, color='#170e14', edge='hills', props=[dict(type='broad', every=.3, h=.5, p=.8, crown='#4a1a14'), dict(type='grass', every=.03, h=.05, color='#3a2018')], fog=0)],
   particles=[dict(kind='leaf', n=70, color=['#c4502a', '#d8902a', '#a8301a'], y0=.05, y1=1, size=(2, 4)), dict(kind='glow', n=20, color='#ffb860', y0=.5, y1=.9, size=(1, 2))]),
 B(id='storm_cliffs', frame=[dict(type='rock', side=-1, h=.18), dict(type='grass', side=1, h=.25)],  name='Storm cliffs', seed=27, sky=['#05070e', '#101624', '#222a3a'], stars=0, rim='#a8c0ff',
   fog='#3a4658', fog_amt=.3, clouds=[dict(y=.08, color='#1a2030', n=8, op=.9), dict(y=.2, color='#242c3e', n=7, op=.7)], layers=[
    dict(base=.66, amp=.0, color='#141c28', edge='none', water_before=dict(y=.66, color='#0c1420')),
    dict(base=.86, amp=.6, color='#0e1118', edge='cliff', ek=dict(side=1, at=.6), pts=70, rimmix=.12, props=[dict(type='rock', every=.08, h=.05, p=.5)]),
    dict(base=.94, amp=.04, color='#0c0f16', edge='hills', props=[dict(type='rock', every=.2, h=.06), dict(type='grass', every=.05, h=.05)], fog=0)],
   extras=[dict(type='lightning', x=.32, to=.6), dict(type='surf', y=.67, x0=.0, x1=1.0)],
   particles=[dict(kind='streak', n=300, color='#a8b8d0', y0=0, y1=1, size=(1, 2), len=18, angle=1.95)]),
 B(id='flower_meadow', frame=[dict(type='flower', side=-1, h=.8, petal='#3a3450', glow=None), dict(type='grass', side=1, h=.3)],  name='Moonlit flower meadow', seed=28, sky=['#0a0c22', '#1c1e44', '#3a3460'], moon=(.5, .2, .1, '#f4f0ff'), stars=200,
   fog='#5a5480', fog_amt=.3, layers=[
    dict(base=.6, amp=.08, color='#26284a', edge='hills', ek=dict(freq=2), props=[dict(type='broad', every=.12, h=.08)]),
    dict(base=.72, amp=.05, color='#1a1c36', edge='hills', props=[dict(type='flower', every=.06, h=.18, petal='#d8d0ff', glow='#c8c0ff')]),
    dict(base=.86, amp=.03, color='#10111f', edge='hills', props=[dict(type='flower', every=.2, h=.42, p=.8, petal='#f0e0ff', glow='#e0d0ff'), dict(type='grass', every=.02, h=.06)], fog=0)],
   extras=[dict(type='rays', x=.5, y=.2, a=95, n=6, color='#d8d0ff')],
   particles=[dict(kind='glow', n=50, color=['#f0e8ff', '#fff0c0'], y0=.4, y1=.95, size=(.8, 1.8))]),
]

if __name__ == '__main__':
    out = sys.argv[1]; want = set(sys.argv[2:])
    for mode in ('card', 'battle'): os.makedirs(os.path.join(out, mode), exist_ok=True)
    for b in BIOMES:
        if want and b['id'] not in want: continue
        for mode in ('card', 'battle'):
            with open(os.path.join(out, mode, b['id'] + '.svg'), 'w') as f: f.write(render(b, mode))
        print(b['id'])
