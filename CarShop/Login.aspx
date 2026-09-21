<%@ Page Title="התחברות" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* ===== Login card (Light Theme) ===== */
        .lg-card {
            max-width: 460px;
            margin: 40px auto;
            padding: 32px 28px 26px;
            direction: rtl;
            font-family: 'Segoe UI', Tahoma, Arial, sans-serif;
            color: #334155; /* Dark gray text */
            background: #ffffff; /* White background */
            border: 1px solid #e2e8f0;
            border-radius: 20px;
            box-shadow: 0 12px 32px rgba(0, 0, 0, 0.08); /* Softer shadow */
            box-sizing: border-box;
        }
        .lg-card *, .lg-card *::before, .lg-card *::after { box-sizing: border-box; }

        .lg-title { margin: 0 0 4px; font-size: 30px; font-weight: 800; color: #0f172a; letter-spacing: -.01em; }
        .lg-sub { margin: 0 0 26px; font-size: 14px; color: #64748b; }

        .lg-group { margin-bottom: 18px; }
        .lg-label { display: block; margin-bottom: 8px; font-size: 14px; font-weight: 600; color: #475569; }

        .lg-input-wrap { position: relative; }
        .lg-input {
            width: 100%;
            padding: 13px 16px;
            font-size: 16px;
            color: #0f172a;
            background: #ffffff;
            border: 2px solid #cbd5e1; /* Light gray border */
            border-radius: 12px;
            transition: border-color .2s, box-shadow .2s;
        }
        .lg-input::placeholder { color: #94a3b8; }
        .lg-input:focus { outline: none; border-color: #64748b; box-shadow: 0 0 0 4px rgba(100, 116, 139, .15); }
        .lg-has-tools { padding-left: 90px; }

        /* Hit reaction */
        .lg-input.hit { animation: lg-hit .45s ease-out; }
        @keyframes lg-hit {
            0%   { transform: translateX(0);    border-color: #94a3b8; box-shadow: 0 0 0 0 rgba(100, 116, 139, .4); }
            25%  { transform: translateX(-4px); }
            50%  { transform: translateX(3px); }
            75%  { transform: translateX(-2px); }
            100% { transform: translateX(0);    box-shadow: 0 0 0 14px rgba(100, 116, 139, 0); }
        }

        /* Buttons inside the password field */
        .lg-tools { position: absolute; left: 8px; top: 50%; transform: translateY(-50%); display: flex; gap: 2px; }
        .lg-icon-btn {
            width: 36px; height: 36px;
            display: grid; place-items: center;
            padding: 0; border: 0; border-radius: 10px;
            background: transparent; color: #64748b; cursor: pointer;
            transition: background .2s, color .2s;
        }
        .lg-icon-btn:hover { background: #f1f5f9; color: #0f172a; }
        .lg-icon-btn svg { width: 20px; height: 20px; fill: none; stroke: currentColor; stroke-width: 2; stroke-linecap: round; stroke-linejoin: round; }

        .lg-card button:focus-visible, .lg-btn:focus-visible { outline: 2px solid #64748b; outline-offset: 2px; }

        /* Digit keypad */
        .lg-pad-head { display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px; font-size: 13px; color: #64748b; }
        .lg-link-btn { padding: 2px 4px; border: 0; background: none; color: #475569; font-size: 13px; cursor: pointer; font-weight: 600; }
        .lg-link-btn:hover { text-decoration: underline; color: #0f172a; }
        .lg-pad { display: grid; grid-template-columns: repeat(5, 1fr); gap: 8px; }
        .lg-digit {
            height: 42px;
            font-size: 17px; font-weight: 700;
            color: #475569;
            background: #ffffff;
            border: 1px solid #cbd5e1;
            border-radius: 12px;
            cursor: pointer;
            transition: transform .15s, background .2s, box-shadow .2s, color .2s;
        }
        .lg-digit:hover { background: #f8fafc; transform: translateY(-2px); border-color: #94a3b8; }
        .lg-digit.selected {
            color: #fff;
            background: var(--c); /* Uses the JS muted color */
            border-color: transparent;
            box-shadow: 0 4px 10px -2px var(--c);
            transform: translateY(-2px) scale(1.04);
        }

        /* Slingshot Area */
        .lg-sling {
            margin-top: 16px;
            padding: 12px 12px 14px;
            text-align: center;
            background: #f8fafc; /* Very light gray */
            border: 1px solid #e2e8f0;
            border-radius: 16px;
            user-select: none; -webkit-user-select: none;
        }
        .lg-sling svg { display: block; width: 100%; max-width: 300px; height: auto; margin: 0 auto; touch-action: none; overflow: visible; }
        #lgHit { cursor: grab; }
        .lg-dragging #lgHit { cursor: grabbing; }
        #lgBall { transform-box: fill-box; transform-origin: center; }
        .lg-pulse { transform-box: fill-box; transform-origin: center; animation: lg-spin 9s linear infinite; }
        @keyframes lg-spin { to { transform: rotate(360deg); } }

        .lg-power { direction: ltr; height: 6px; max-width: 220px; margin: 6px auto 0; overflow: hidden; background: #e2e8f0; border-radius: 99px; }
        /* Muted power bar colors */
        .lg-power > i { display: block; height: 100%; background: linear-gradient(90deg, #94a3b8, #64748b, #475569); clip-path: inset(0 100% 0 0); }
        .lg-hint { min-height: 18px; margin: 10px 0 0; font-size: 13px; color: #64748b; transition: opacity .4s; }

        /* Error + submit */
        .lg-error {
            display: block; margin-top: 16px; padding: 10px 12px;
            font-size: 14px; font-weight: 600; text-align: center;
            color: #b91c1c; background: #fef2f2;
            border: 1px solid #fecaca; border-radius: 10px;
        }
        .lg-error:empty { display: none; }

        .lg-btn {
            width: 100%; margin-top: 18px; padding: 14px;
            font-size: 17px; font-weight: 700; color: #fff;
            background: #475569; /* Muted slate button */
            border: 0; border-radius: 12px; cursor: pointer;
            transition: background .2s, transform .15s;
        }
        .lg-btn:hover { background: #334155; }
        .lg-btn:active { transform: translateY(1px); }

        .lg-foot { margin: 20px 0 0; text-align: center; font-size: 14px; color: #64748b; }
        .lg-foot a { color: #475569; font-weight: 600; text-decoration: none; }
        .lg-foot a:hover { text-decoration: underline; color: #0f172a; }

        /* Flying elements */
        .lg-traj { position: fixed; left: 0; top: 0; width: 100%; height: 100%; pointer-events: none; z-index: 99997; }
        .lg-dot { position: fixed; left: 0; top: 0; border-radius: 50%; pointer-events: none; z-index: 99998; }
        .lg-ring { width: 24px; height: 24px; margin: -12px 0 0 -12px; border: 3px solid; background: transparent; }
        .lg-spark { width: 7px; height: 7px; margin: -3.5px 0 0 -3.5px; }
        .lg-trail { width: 12px; height: 12px; margin: -6px 0 0 -6px; }
        .lg-fly {
            position: fixed; left: 0; top: 0;
            width: 44px; height: 44px; margin: -22px 0 0 -22px;
            display: grid; place-items: center;
            border-radius: 50%;
            font: 800 22px 'Segoe UI', Arial, sans-serif; color: #fff;
            background: radial-gradient(circle at 32% 28%, rgba(255, 255, 255, .6), rgba(255, 255, 255, 0) 45%), var(--c);
            box-shadow: 0 6px 12px rgba(0, 0, 0, .15), inset -4px -6px 10px rgba(0, 0, 0, .1);
            pointer-events: none; z-index: 99999; will-change: transform;
        }

        @media (prefers-reduced-motion: reduce) {
            .lg-pulse, .lg-input.hit { animation: none; }
        }
        @media (max-width: 480px) {
            .lg-card { margin: 16px 12px; padding: 24px 18px 20px; }
        }
    </style>

    <div class="lg-card">
        <h2 class="lg-title">התחברות</h2>
        <p class="lg-sub">התחבר כדי להמשיך</p>

        <div class="lg-group">
            <label class="lg-label">שם משתמש</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="lg-input" placeholder="הקלד שם משתמש..." autocomplete="username"></asp:TextBox>
        </div>

        <div class="lg-group">
            <label class="lg-label" for="txtPassword">סיסמה</label>
            <div class="lg-input-wrap">
                <asp:TextBox ID="txtPassword" ClientIDMode="Static" runat="server" TextMode="Password" CssClass="lg-input lg-has-tools" placeholder="הסיסמה תשוגר לכאן..." autocomplete="current-password"></asp:TextBox>
                <div class="lg-tools">
                    <button type="button" id="lgBack" class="lg-icon-btn" title="מחק ספרה אחרונה" aria-label="מחק ספרה אחרונה">
                        <svg viewBox="0 0 24 24"><path d="M21 4H8l-7 8 7 8h13a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2z"/><line x1="18" y1="9" x2="12" y2="15"/><line x1="12" y1="9" x2="18" y2="15"/></svg>
                    </button>
                    <button type="button" id="lgEye" class="lg-icon-btn" title="הצג סיסמה" aria-label="הצג סיסמה" aria-pressed="false">
                        <svg id="lgEyeOn" viewBox="0 0 24 24"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
                        <svg id="lgEyeOff" viewBox="0 0 24 24" style="display:none"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>
                    </button>
                </div>
            </div>
        </div>

        <div class="lg-pad-head">
            <span>בחר ספרה לטעינה</span>
            <button type="button" id="lgClear" class="lg-link-btn">נקה הכל</button>
        </div>
        <div class="lg-pad" id="lgPad"></div>

        <!-- Slingshot -->
        <div class="lg-sling">
            <svg id="lgSling" viewBox="0 0 300 250" role="img" aria-label="רוגטקה לירי ספרות">
                <defs>
                    <radialGradient id="lgShade" cx=".35" cy=".3" r=".85">
                        <stop offset="0" stop-color="#fff" stop-opacity=".35"/>
                        <stop offset=".55" stop-color="#fff" stop-opacity="0"/>
                        <stop offset="1" stop-color="#000" stop-opacity=".15"/> <!-- Lighter shadow inside ball -->
                    </radialGradient>
                </defs>

                <ellipse cx="150" cy="242" rx="52" ry="6" fill="rgba(0,0,0,.06)"/> <!-- Softer floor shadow -->

                <!-- Muted wooden frame -->
                <g fill="none" stroke-linecap="round">
                    <path d="M150 240 L150 148" stroke="#a39b8e" stroke-width="20"/>
                    <path d="M150 160 C150 118 92 118 90 62" stroke="#a39b8e" stroke-width="16"/>
                    <path d="M150 160 C150 118 208 118 210 62" stroke="#a39b8e" stroke-width="16"/>
                    <path d="M146 236 L146 152" stroke="#c2baba" stroke-width="4" opacity=".6"/>
                    <path d="M146 158 C146 120 88 118 86 64" stroke="#c2baba" stroke-width="3.5" opacity=".6"/>
                    <path d="M154 158 C154 120 204 118 206 64" stroke="#c2baba" stroke-width="3.5" opacity=".4"/>
                </g>
                <rect x="139" y="184" width="22" height="38" rx="6" fill="#64748b"/>
                <g stroke="#94a3b8" stroke-width="2"><line x1="139" y1="194" x2="161" y2="190"/><line x1="139" y1="204" x2="161" y2="200"/><line x1="139" y1="214" x2="161" y2="210"/></g>
                <circle cx="90" cy="62" r="10" fill="#9ca3af"/>
                <circle cx="210" cy="62" r="10" fill="#9ca3af"/>

                <!-- Gray rubber band -->
                <line id="lgBandL" x1="90" y1="62" x2="134" y2="89" stroke="#9ca3af" stroke-width="8" stroke-linecap="round"/>

                <!-- Pouch + ball -->
                <g id="lgPouch" transform="translate(150,86)">
                    <circle id="lgPulse" class="lg-pulse" r="33" fill="none" stroke="rgba(148,163,184,.6)" stroke-width="2" stroke-dasharray="5 7"/>
                    <ellipse cx="0" cy="10" rx="28" ry="13" fill="#cbd5e1" stroke="#94a3b8" stroke-width="2"/> <!-- Light pouch -->
                    <g id="lgBall">
                        <circle id="lgBallBase" r="22" fill="hsl(210, 20%, 50%)"/> <!-- Muted start color -->
                        <circle r="22" fill="url(#lgShade)"/>
                        <ellipse cx="-8" cy="-9" rx="7" ry="4" fill="rgba(255,255,255,.55)" transform="rotate(-35 -8 -9)"/>
                        <text id="lgBallText" y="1" text-anchor="middle" dominant-baseline="central" font-size="24" font-weight="800" fill="#fff" stroke="rgba(0,0,0,.15)" stroke-width="1.5" style="paint-order:stroke">1</text>
                    </g>
                    <circle id="lgHit" r="38" fill="transparent"/>
                </g>

                <!-- front band -->
                <line id="lgBandR" x1="210" y1="62" x2="166" y2="89" stroke="#9ca3af" stroke-width="8" stroke-linecap="round"/>
            </svg>
            <div class="lg-power" aria-hidden="true"><i id="lgPowerFill"></i></div>
            <p class="lg-hint" id="lgHint">משוך את הכדור כלפי מטה ושחרר לירי</p>
        </div>

        <asp:Label ID="lblError" runat="server" CssClass="lg-error"></asp:Label>

        <asp:Button ID="btnLogin" ClientIDMode="Static" runat="server" Text="התחבר למערכת" CssClass="lg-btn" OnClick="btnLogin_Click" />

        <p class="lg-foot">עדיין אין לך חשבון? <asp:HyperLink ID="hlRegister" runat="server" NavigateUrl="~/Register.aspx">הירשם כאן</asp:HyperLink></p>
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const NS = 'http://www.w3.org/2000/svg';
            const $ = id => document.getElementById(id);

            const svg = $('lgSling'), pouch = $('lgPouch'), ballG = $('lgBall'),
                  ballBase = $('lgBallBase'), ballText = $('lgBallText'),
                  hitArea = $('lgHit'), pulse = $('lgPulse'),
                  bandL = $('lgBandL'), bandR = $('lgBandR'),
                  pad = $('lgPad'), pw = $('txtPassword'),
                  powerFill = $('lgPowerFill'), hint = $('lgHint');

            const ORIGIN = { x: 150, y: 86 };
            const MAX_PULL = 78;
            const MIN_POWER = 0.22;
            const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

            let digit = '1', loaded = true, dragging = false;
            let grab = { x: 0, y: 0 }, pos = { x: ORIGIN.x, y: ORIGIN.y };
            let springRaf = 0, hintTimer = 0;

            // ---------- UPDATED COLOR LOGIC ----------
            // Creates soft, desaturated, pastel-like shades (low saturation, medium lightness)
            const color = d => 'hsl(' + (200 + parseInt(d, 10) * 15) + ', 22%, 52%)';
            
            const toSvg = (x, y) => new DOMPoint(x, y).matrixTransform(svg.getScreenCTM().inverse());
            const toScreen = (x, y) => new DOMPoint(x, y).matrixTransform(svg.getScreenCTM());
            const targetPoint = () => {
                const r = pw.getBoundingClientRect();
                return { x: r.left + r.width / 2, y: r.top + r.height / 2 };
            };
            const lateralOf = () => -(pos.x - ORIGIN.x) * 1.6;

            function say(msg, keep) {
                hint.textContent = msg;
                hint.style.opacity = 1;
                clearTimeout(hintTimer);
                if (!keep) hintTimer = setTimeout(function () { hint.style.opacity = 0; }, 2200);
            }

            function arc(P0, P2, power, lateral) {
                const P1 = { x: (P0.x + P2.x) / 2 + lateral, y: Math.min(P0.y, P2.y) - 60 - power * 90 };
                return function (t) {
                    const u = 1 - t;
                    return {
                        x: u * u * P0.x + 2 * u * t * P1.x + t * t * P2.x,
                        y: u * u * P0.y + 2 * u * t * P1.y + t * t * P2.y
                    };
                };
            }

            function setPos(x, y) {
                pos.x = x; pos.y = y;
                pouch.setAttribute('transform', 'translate(' + x + ',' + y + ')');
                const p = Math.min(1, Math.hypot(x - ORIGIN.x, y - ORIGIN.y) / MAX_PULL);
                const w = (8 - 4 * p).toFixed(2);
                
                // Muted band tightening color (stays gray/slate)
                const c = 'hsl(215, ' + (15 + 15 * p) + '%, ' + (65 - 10 * p) + '%)'; 
                
                [[bandL, x - 16], [bandR, x + 16]].forEach(function (item) {
                    item[0].setAttribute('x2', item[1]);
                    item[0].setAttribute('y2', y + 3);
                    item[0].setAttribute('stroke-width', w);
                    item[0].setAttribute('stroke', c);
                });
                return p;
            }

            function setPower(p) {
                powerFill.style.clipPath = 'inset(0 ' + ((1 - p) * 100) + '% 0 0)';
            }

            function springBack() {
                cancelAnimationFrame(springRaf);
                const sx = pos.x - ORIGIN.x, sy = pos.y - ORIGIN.y;
                const t0 = performance.now();
                function step(now) {
                    const t = (now - t0) / 1000;
                    if (t >= 0.75) { setPos(ORIGIN.x, ORIGIN.y); return; }
                    const k = Math.exp(-7 * t) * Math.cos(26 * t);
                    setPos(ORIGIN.x + sx * k, ORIGIN.y + sy * k);
                    springRaf = requestAnimationFrame(step);
                }
                springRaf = requestAnimationFrame(step);
            }

            function selectDigit(d, btn) {
                digit = d;
                ballText.textContent = d;
                ballBase.setAttribute('fill', color(d));
                pad.querySelectorAll('.lg-digit').forEach(function (b) { b.classList.remove('selected'); });
                if (btn) btn.classList.add('selected');
                if (loaded) {
                    ballG.animate(
                        [{ transform: 'scale(.8)' }, { transform: 'scale(1.12)' }, { transform: 'scale(1)' }],
                        { duration: 220, easing: 'ease-out' }
                    );
                }
            }

            ['1', '2', '3', '4', '5', '6', '7', '8', '9', '0'].forEach(function (d) {
                const b = document.createElement('button');
                b.type = 'button';
                b.className = 'lg-digit' + (d === '1' ? ' selected' : '');
                b.textContent = d;
                b.setAttribute('aria-label', 'בחר ספרה ' + d);
                b.style.setProperty('--c', color(d));
                b.addEventListener('click', function () { selectDigit(d, b); });
                pad.appendChild(b);
            });
            selectDigit('1', pad.querySelector('.lg-digit'));

            const traj = document.createElementNS(NS, 'svg');
            traj.setAttribute('class', 'lg-traj');
            traj.style.display = 'none';
            const DOTS = 14, dots = [];
            for (let i = 0; i < DOTS; i++) {
                const c = document.createElementNS(NS, 'circle');
                traj.appendChild(c);
                dots.push(c);
            }
            document.body.appendChild(traj);

            function drawTrajectory(power) {
                if (power < MIN_POWER) { traj.style.display = 'none'; return; }
                traj.style.display = '';
                const f = arc(toScreen(pos.x, pos.y), targetPoint(), power, lateralOf());
                const col = color(digit);
                dots.forEach(function (dot, i) {
                    const p = f(((i + 1) / (DOTS + 1)) * 0.98);
                    dot.setAttribute('cx', p.x);
                    dot.setAttribute('cy', p.y);
                    dot.setAttribute('r', (4 - i * 0.18).toFixed(2));
                    dot.setAttribute('fill', col);
                    dot.setAttribute('opacity', (0.8 - i * 0.05).toFixed(2));
                });
            }

            function addDigit(d) {
                pw.value += d;
                pw.dispatchEvent(new Event('input', { bubbles: true }));
            }

            function impact(pt, d) {
                addDigit(d);
                pw.classList.remove('hit');
                void pw.offsetWidth;
                pw.classList.add('hit');
                if (navigator.vibrate) navigator.vibrate(15);
                if (reduceMotion) return;

                const c = color(d);
                const ring = document.createElement('div');
                ring.className = 'lg-dot lg-ring';
                ring.style.borderColor = c;
                document.body.appendChild(ring);
                ring.animate([
                    { transform: 'translate(' + pt.x + 'px,' + pt.y + 'px) scale(.4)', opacity: 0.7 },
                    { transform: 'translate(' + pt.x + 'px,' + pt.y + 'px) scale(3.5)', opacity: 0 }
                ], { duration: 520, easing: 'ease-out', fill: 'forwards' }).onfinish = function () { ring.remove(); };

                for (let i = 0; i < 10; i++) {
                    const a = Math.random() * Math.PI * 2, dist = 24 + Math.random() * 36;
                    const s = document.createElement('div');
                    s.className = 'lg-dot lg-spark';
                    s.style.background = c;
                    document.body.appendChild(s);
                    const x1 = pt.x + Math.cos(a) * dist, y1 = pt.y + Math.sin(a) * dist + 14;
                    s.animate([
                        { transform: 'translate(' + pt.x + 'px,' + pt.y + 'px) scale(1)', opacity: 0.8 },
                        { transform: 'translate(' + x1 + 'px,' + y1 + 'px) scale(.2)', opacity: 0 }
                    ], { duration: 400 + Math.random() * 300, easing: 'cubic-bezier(.2,.7,.3,1)', fill: 'forwards' })
                     .onfinish = function () { s.remove(); };
                }
            }

            function launch(from, power, d, lateral) {
                const P2 = targetPoint();
                if (reduceMotion) { impact(P2, d); return; }

                const f = arc(from, P2, power, lateral);
                const fly = document.createElement('div');
                fly.className = 'lg-fly';
                fly.textContent = d;
                fly.style.setProperty('--c', color(d));
                document.body.appendChild(fly);

                const dur = 620 - power * 220;
                const t0 = performance.now();
                let lastTrail = 0;

                function step(now) {
                    const k = Math.min(1, (now - t0) / dur);
                    const t = 1 - Math.pow(1 - k, 1.5);
                    const p = f(t);
                    const scale = 1.1 - 0.5 * t;
                    fly.style.transform = 'translate(' + p.x + 'px,' + p.y + 'px) scale(' + scale + ') rotate(' + (t * 360) + 'deg)';

                    if (now - lastTrail > 32 && k < 1) {
                        lastTrail = now;
                        const tr = document.createElement('div');
                        tr.className = 'lg-dot lg-trail';
                        tr.style.background = color(d);
                        document.body.appendChild(tr);
                        tr.animate([
                            { transform: 'translate(' + p.x + 'px,' + p.y + 'px) scale(' + scale + ')', opacity: 0.4 },
                            { transform: 'translate(' + p.x + 'px,' + p.y + 'px) scale(0.1)', opacity: 0 }
                        ], { duration: 350, easing: 'ease-out', fill: 'forwards' }).onfinish = function () { tr.remove(); };
                    }

                    if (k < 1) {
                        requestAnimationFrame(step);
                    } else {
                        fly.remove();
                        impact(P2, d);
                    }
                }
                requestAnimationFrame(step);
            }

            function reload() {
                loaded = true;
                ballG.style.display = '';
                ballG.animate(
                    [{ transform: 'scale(0)' }, { transform: 'scale(1.15)' }, { transform: 'scale(1)' }],
                    { duration: 320, easing: 'ease-out' }
                );
            }

            function fire(power) {
                const from = toScreen(pos.x, pos.y);
                const lateral = lateralOf();
                const d = digit;
                loaded = false;
                ballG.style.display = 'none';
                springBack();
                setTimeout(reload, 380);
                launch(from, power, d, lateral);
            }

            hitArea.addEventListener('pointerdown', function (e) {
                if (!loaded) return;
                e.preventDefault();
                cancelAnimationFrame(springRaf);
                dragging = true;
                svg.setPointerCapture(e.pointerId);
                svg.classList.add('lg-dragging');
                const s = toSvg(e.clientX, e.clientY);
                grab = { x: pos.x - s.x, y: pos.y - s.y };
                pulse.style.display = 'none';
                hint.style.opacity = 0;
            });

            svg.addEventListener('pointermove', function (e) {
                if (!dragging) return;
                const s = toSvg(e.clientX, e.clientY);
                let dx = s.x + grab.x - ORIGIN.x;
                let dy = s.y + grab.y - ORIGIN.y;
                if (dy < 0) dy = 0;
                const dist = Math.hypot(dx, dy);
                if (dist > MAX_PULL) { dx *= MAX_PULL / dist; dy *= MAX_PULL / dist; }
                const p = setPos(ORIGIN.x + dx, ORIGIN.y + dy);
                setPower(p);
                drawTrajectory(p);
            });

            function release() {
                if (!dragging) return;
                dragging = false;
                svg.classList.remove('lg-dragging');
                traj.style.display = 'none';
                const p = Math.min(1, Math.hypot(pos.x - ORIGIN.x, pos.y - ORIGIN.y) / MAX_PULL);
                setPower(0);
                if (p < MIN_POWER) {
                    springBack();
                    say('משוך קצת יותר חזק');
                } else {
                    fire(p);
                }
            }
            svg.addEventListener('pointerup', release);
            svg.addEventListener('pointercancel', release);

            $('lgBack').addEventListener('click', function () {
                pw.value = pw.value.slice(0, -1);
            });
            $('lgClear').addEventListener('click', function () {
                pw.value = '';
            });
            $('lgEye').addEventListener('click', function () {
                const show = pw.type === 'password';
                pw.type = show ? 'text' : 'password';
                $('lgEyeOn').style.display = show ? 'none' : '';
                $('lgEyeOff').style.display = show ? '' : 'none';
                this.setAttribute('aria-pressed', show ? 'true' : 'false');
                const label = show ? 'הסתר סיסמה' : 'הצג סיסמה';
                this.setAttribute('aria-label', label);
                this.title = label;
            });

            setPos(ORIGIN.x, ORIGIN.y);
        });
    </script>
</asp:Content>