// script.js - VoidMemory Website Interactions

document.addEventListener('DOMContentLoaded', function() {
    // ========================
    // 1. TERMINAL DEMO RESET FUNCTIONALITY
    // ========================
    const resetDemoBtn = document.getElementById('resetDemoBtn');
    const terminalBody = document.getElementById('terminalBody');
    
    if (resetDemoBtn && terminalBody) {
        const originalTerminalContent = terminalBody.innerHTML;
        
        resetDemoBtn.addEventListener('click', function() {
            terminalBody.innerHTML = originalTerminalContent;
            
            const lines = terminalBody.querySelectorAll('.line');
            if (lines.length > 0) {
                const lastLine = lines[lines.length - 1];
                if (lastLine && !lastLine.querySelector('.blink-cursor')) {
                    const cursorSpan = document.createElement('span');
                    cursorSpan.className = 'blink-cursor';
                    lastLine.appendChild(cursorSpan);
                }
            }
        });
    }
    
    // ========================
    // 2. TAB SYSTEM FOR QUICK START GUIDE
    // ========================
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabPanes = document.querySelectorAll('.tab-pane');
    
    if (tabBtns.length > 0 && tabPanes.length > 0) {
        tabBtns.forEach(btn => {
            btn.addEventListener('click', function() {
                const targetTab = this.getAttribute('data-tab');
                
                tabBtns.forEach(btn => btn.classList.remove('active'));
                tabPanes.forEach(pane => pane.classList.remove('active'));
                
                this.classList.add('active');
                const activePane = document.getElementById(targetTab);
                if (activePane) {
                    activePane.classList.add('active');
                }
            });
        });
    }
    
    // ========================
    // 3. FORK BUTTON (SIMULATED GITHUB FORK)
    // ========================
    const forkBtn = document.getElementById('forkBtn');
    if (forkBtn) {
        forkBtn.addEventListener('click', function(e) {
            e.preventDefault();
            window.open('https://github.com/CzaxStudio/VoidMemory/fork', '_blank');
        });
    }
    
    // ========================
    // 4. CONTRIBUTE BUTTON (SIMULATED)
    // ========================
    const contributeBtn = document.getElementById('contributeIdea');
    if (contributeBtn) {
        contributeBtn.addEventListener('click', function(e) {
            e.preventDefault();
            showToastNotification('Check the GitHub repository for contribution guidelines!', 'info');
        });
    }
    
    // ========================
    // 5. TOAST NOTIFICATION SYSTEM
    // ========================
    function showToastNotification(message, type = 'info') {
        const existingToast = document.querySelector('.toast-notification');
        if (existingToast) {
            existingToast.remove();
        }
        
        const toast = document.createElement('div');
        toast.className = `toast-notification toast-${type}`;
        
        let iconHtml = '';
        if (type === 'info') iconHtml = '<i class="fas fa-info-circle"></i>';
        if (type === 'success') iconHtml = '<i class="fas fa-check-circle"></i>';
        if (type === 'warning') iconHtml = '<i class="fas fa-exclamation-triangle"></i>';
        
        toast.innerHTML = `
            <div class="toast-content">
                ${iconHtml}
                <span>${message}</span>
            </div>
            <button class="toast-close">&times;</button>
        `;
        
        document.body.appendChild(toast);
        
        const closeBtn = toast.querySelector('.toast-close');
        if (closeBtn) {
            closeBtn.addEventListener('click', () => {
                toast.remove();
            });
        }
        
        setTimeout(() => {
            if (toast && toast.parentNode) {
                toast.classList.add('toast-fade-out');
                setTimeout(() => {
                    if (toast && toast.parentNode) toast.remove();
                }, 300);
            }
        }, 4000);
        
        setTimeout(() => {
            toast.classList.add('toast-show');
        }, 10);
    }
    
    // ========================
    // 6. ADD TOAST STYLES DYNAMICALLY
    // ========================
    const toastStyles = document.createElement('style');
    toastStyles.textContent = `
        .toast-notification {
            position: fixed;
            bottom: 24px;
            right: 24px;
            background: var(--bg-elevated, #1a1a2a);
            border-left: 4px solid var(--primary, #6b21ff);
            border-radius: 12px;
            padding: 0.9rem 1.2rem;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 1rem;
            min-width: 280px;
            max-width: 400px;
            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.4);
            z-index: 10000;
            transform: translateX(120%);
            transition: transform 0.3s ease;
            font-family: var(--font-sans, 'Inter', sans-serif);
            backdrop-filter: blur(12px);
            background: rgba(26, 26, 42, 0.95);
            border: 1px solid var(--border-subtle, #2a2a3a);
        }
        
        .toast-notification.toast-show {
            transform: translateX(0);
        }
        
        .toast-notification.toast-fade-out {
            transform: translateX(120%);
        }
        
        .toast-content {
            display: flex;
            align-items: center;
            gap: 0.75rem;
            color: var(--text-primary, #f0f0f5);
            font-size: 0.85rem;
        }
        
        .toast-content i {
            font-size: 1.1rem;
        }
        
        .toast-info {
            border-left-color: var(--primary, #6b21ff);
        }
        
        .toast-success {
            border-left-color: var(--success, #00c853);
        }
        
        .toast-warning {
            border-left-color: var(--warning, #ffab00);
        }
        
        .toast-close {
            background: transparent;
            border: none;
            color: var(--text-muted, #6a6a7a);
            font-size: 1.2rem;
            cursor: pointer;
            padding: 0;
            line-height: 1;
            transition: color 0.2s;
        }
        
        .toast-close:hover {
            color: var(--text-primary, #f0f0f5);
        }
        
        @media (max-width: 640px) {
            .toast-notification {
                left: 16px;
                right: 16px;
                bottom: 16px;
                min-width: auto;
                max-width: none;
            }
        }
    `;
    document.head.appendChild(toastStyles);
    
    // ========================
    // 7. SMOOTH SCROLLING FOR ANCHOR LINKS
    // ========================
    const allLinks = document.querySelectorAll('a[href^="#"]');
    allLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            const targetId = this.getAttribute('href');
            if (targetId && targetId !== '#') {
                const targetElement = document.querySelector(targetId);
                if (targetElement) {
                    e.preventDefault();
                    targetElement.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });
    
    // ========================
    // 8. GLITCH EFFECT ON HERO TITLE (HOVER)
    // ========================
    const glitchElement = document.querySelector('.glitch');
    if (glitchElement) {
        glitchElement.addEventListener('mouseenter', function() {
            this.style.animation = 'glitchSkew 0.3s infinite';
        });
        
        glitchElement.addEventListener('mouseleave', function() {
            this.style.animation = '';
        });
        
        const glitchKeyframes = document.createElement('style');
        glitchKeyframes.textContent = `
            @keyframes glitchSkew {
                0% { transform: skew(0deg); text-shadow: -2px 0 var(--primary), 2px 0 var(--accent-cyan); }
                20% { transform: skew(2deg); text-shadow: 2px 0 var(--primary), -2px 0 var(--accent-cyan); }
                40% { transform: skew(-2deg); }
                60% { transform: skew(1deg); }
                80% { transform: skew(-1deg); }
                100% { transform: skew(0deg); }
            }
        `;
        document.head.appendChild(glitchKeyframes);
    }
    
    // ========================
    // 9. ADD TOOLTIPS FOR TECHNICAL TERMS
    // ========================
    const tooltipElements = document.querySelectorAll('.build-cmd, code');
    tooltipElements.forEach(el => {
        if (!el.hasAttribute('title')) {
            if (el.classList.contains('build-cmd')) {
                el.setAttribute('title', 'Build command for C# source code');
            } else if (el.textContent.includes('OpenProcess') || el.textContent.includes('ReadProcessMemory')) {
                el.setAttribute('title', 'Windows API functions for memory access');
            } else {
                el.setAttribute('title', 'Technical reference');
            }
        }
    });
    
    // ========================
    // 10. INTERACTIVE TERMINAL COMMAND SIMULATION (CLICK ON COMMANDS)
    // ========================
    const cmdElements = document.querySelectorAll('.cmd');
    cmdElements.forEach(cmd => {
        cmd.style.cursor = 'pointer';
        cmd.addEventListener('click', function(e) {
            e.stopPropagation();
            const commandText = this.textContent;
            showToastNotification(`Demo: ${commandText} (simulated)`, 'info');
            
            const newOutputLine = document.createElement('div');
            newOutputLine.className = 'line output';
            newOutputLine.textContent = '[DEMO] Command received. Real tool would execute this operation.';
            
            if (terminalBody) {
                terminalBody.appendChild(newOutputLine);
                terminalBody.scrollTop = terminalBody.scrollHeight;
                
                setTimeout(() => {
                    const cursor = terminalBody.querySelector('.blink-cursor');
                    if (cursor && cursor.parentNode) {
                        cursor.remove();
                    }
                }, 100);
            }
        });
    });
    
    // ========================
    // 11. ADD COPY FUNCTIONALITY FOR BUILD COMMANDS
    // ========================
    const buildCommandElement = document.querySelector('.build-cmd');
    if (buildCommandElement) {
        buildCommandElement.style.cursor = 'pointer';
        buildCommandElement.addEventListener('click', function() {
            const commandText = this.textContent;
            navigator.clipboard.writeText(commandText).then(() => {
                showToastNotification('Build command copied to clipboard!', 'success');
            }).catch(() => {
                showToastNotification('Could not copy command', 'warning');
            });
        });
        
        const copyHint = document.createElement('span');
        copyHint.textContent = ' (click to copy)';
        copyHint.style.fontSize = '0.65rem';
        copyHint.style.opacity = '0.6';
        copyHint.style.marginLeft = '0.3rem';
        buildCommandElement.appendChild(copyHint);
    }
    
    // ========================
    // 12. PARALLAX EFFECT ON PREVIEW IMAGE
    // ========================
    const previewImg = document.querySelector('.preview-img');
    if (previewImg) {
        window.addEventListener('mousemove', function(e) {
            const mouseX = e.clientX / window.innerWidth;
            const mouseY = e.clientY / window.innerHeight;
            const moveX = (mouseX - 0.5) * 8;
            const moveY = (mouseY - 0.5) * 8;
            previewImg.style.transform = `translate(${moveX}px, ${moveY}px) scale(1.01)`;
        });
        
        const wrapper = document.querySelector('.image-wrapper');
        if (wrapper) {
            wrapper.addEventListener('mouseleave', function() {
                previewImg.style.transform = 'translate(0, 0) scale(1)';
            });
        }
    }
    
    // ========================
    // 13. DETECT IF USER IS ON DARK MODE (ALREADY DARK, BUT ADD FALLBACK)
    // ========================
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)');
    if (!prefersDark.matches) {
        console.log('VoidMemory theme is optimized for dark mode');
    }
    
    // ========================
    // 14. ADD KEYBOARD SHORTCUT: 'R' TO RESET TERMINAL
    // ========================
    document.addEventListener('keydown', function(e) {
        if (e.key === 'r' || e.key === 'R') {
            if (document.activeElement && document.activeElement.tagName === 'INPUT') {
                return;
            }
            if (resetDemoBtn && terminalBody) {
                e.preventDefault();
                resetDemoBtn.click();
                showToastNotification('Terminal demo reset', 'info');
            }
        }
    });
    
    // ========================
    // 15. DYNAMIC YEAR IN FOOTER (OPTIONAL ENHANCEMENT)
    // ========================
    const copyrightElement = document.querySelector('.copyright p');
    if (copyrightElement) {
        const currentYear = new Date().getFullYear();
        const originalText = copyrightElement.textContent;
        if (!originalText.includes(currentYear.toString())) {
            copyrightElement.textContent = originalText.replace('VoidMemory', `VoidMemory ${currentYear}`);
        }
    }
    
    // ========================
    // 16. SCROLL REVEAL ANIMATION FOR FEATURE CARDS
    // ========================
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const revealObserver = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
                revealObserver.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    const animatedElements = document.querySelectorAll('.feature-card, .info-block, .flow-card');
    animatedElements.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
        revealObserver.observe(el);
    });
    
    // ========================
    // 17. DOWNLOAD BUTTON TRACKING (ANALYTICS SIMULATION)
    // ========================
    const downloadBtn = document.querySelector('.btn-secondary');
    if (downloadBtn) {
        downloadBtn.addEventListener('click', function() {
            console.log('[VoidMemory] Download initiated from website');
        });
    }
    
    // ========================
    // 18. GITHUB STAR COUNTER SIMULATION (STATIC DISPLAY)
    // ========================
    const starLink = document.querySelector('.support-links a:first-child');
    if (starLink) {
        starLink.addEventListener('click', function(e) {
            setTimeout(() => {
                showToastNotification('Thank you for starring VoidMemory!', 'success');
            }, 100);
        });
    }
    
    // ========================
    // 19. RESPONSIVE TERMINAL HEIGHT ADJUSTMENT
    // ========================
    function adjustTerminalHeight() {
        const terminalWindow = document.querySelector('.terminal-window');
        if (terminalWindow && window.innerWidth < 640) {
            const terminalBodyElem = terminalWindow.querySelector('.terminal-body');
            if (terminalBodyElem) {
                terminalBodyElem.style.maxHeight = '300px';
                terminalBodyElem.style.overflowY = 'auto';
            }
        } else if (terminalWindow) {
            const terminalBodyElem = terminalWindow.querySelector('.terminal-body');
            if (terminalBodyElem) {
                terminalBodyElem.style.maxHeight = '';
                terminalBodyElem.style.overflowY = '';
            }
        }
    }
    
    window.addEventListener('resize', adjustTerminalHeight);
    adjustTerminalHeight();
    
    // ========================
    // 20. INITIAL CONSOLE WELCOME MESSAGE
    // ========================
    console.log('%cVoidMemory Website Loaded', 'color: #6b21ff; font-size: 16px; font-weight: bold;');
    console.log('%cControl Memory. Control Reality.', 'color: #00d4ff; font-size: 12px;');
    
    showToastNotification('VoidMemory website ready — check out the features!', 'info');
});
