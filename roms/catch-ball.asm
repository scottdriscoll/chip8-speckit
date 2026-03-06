; Catch Ball
; Controls: A = move left, D = move right
; Score appears in the top-right corner.
; Lives appear in the top-left as three player icons.
; The player sprite stays on the bottom row.

; 0x380: ball sprite (2 bytes)
; 0x382: player sprite (4 bytes)
; 0x386+: custom GAME OVER glyphs

0x200: 00E0  ; CLS
0x202: 6018  ; V0 = player X
0x204: 611C  ; V1 = player Y (bottom row)
0x206: 6408  ; V4 = fall speed delay
0x208: 6600  ; V6 = score ones
0x20A: 6700  ; V7 = score tens
0x20C: 6907  ; V9 = left key nibble (A)
0x20E: 6A09  ; VA = right key nibble (D)
0x210: 6D03  ; VD = lives
0x212: A382  ; I = player sprite
0x214: D014  ; draw player
0x216: 2260  ; draw score
0x218: 2270  ; draw starting lives
0x21A: 2300  ; spawn first ball

0x21C: 2222  ; handle input
0x21E: 2282  ; update ball
0x220: 121C  ; loop

; handle_input
0x222: E99E  ; if left pressed
0x224: 2234  ;   handle left
0x226: E9A1  ; if left not pressed
0x228: 6B00  ;   clear left latch
0x22A: EA9E  ; if right pressed
0x22C: 224A  ;   handle right
0x22E: EAA1  ; if right not pressed
0x230: 6C00  ;   clear right latch
0x232: 00EE  ; return

; move left once per key press
0x234: 3B00  ; if VB == 0 skip next
0x236: 00EE  ; return
0x238: 6B01  ; VB = 1
0x23A: 4000  ; if V0 != 0 skip next
0x23C: 00EE  ; return at left edge
0x23E: A382  ; erase player
0x240: D014
0x242: 70FC  ; V0 -= 4
0x244: A382  ; draw player
0x246: D014
0x248: 00EE

; move right once per key press
0x24A: 3C00  ; if VC == 0 skip next
0x24C: 00EE
0x24E: 6C01  ; VC = 1
0x250: 4038  ; if V0 != 56 skip next
0x252: 00EE  ; return at right edge
0x254: A382  ; erase player
0x256: D014
0x258: 7004  ; V0 += 4
0x25A: A382  ; draw player
0x25C: D014
0x25E: 00EE

; draw or erase score using XOR
0x260: 6E00  ; VE = y = 0
0x262: F729  ; I = font for tens
0x264: 6834  ; V8 = tens x
0x266: D8E5  ; draw tens
0x268: F629  ; I = font for ones
0x26A: 683A  ; V8 = ones x
0x26C: D8E5  ; draw ones
0x26E: 00EE

; draw the three life icons once at startup
0x270: 6E00  ; VE = y = 0
0x272: A382  ; player sprite
0x274: 6800  ; x = 0
0x276: D8E4
0x278: 6806  ; x = 6
0x27A: D8E4
0x27C: 680C  ; x = 12
0x27E: D8E4
0x280: 00EE

; update ball when delay timer reaches zero
0x282: F507  ; V5 = delay timer
0x284: 3500  ; if V5 == 0 skip next
0x286: 00EE  ; return until timer expires
0x288: A380  ; erase ball
0x28A: D232
0x28C: 7301  ; V3 += 1
0x28E: F415  ; reset delay timer from speed
0x290: 5310  ; if ball Y == player Y skip next
0x292: 12B8  ; jump to redraw-or-miss
0x294: 5200  ; if ball X == player X skip next
0x296: 12B8  ; jump to redraw-or-miss
0x298: 2260  ; erase old score
0x29A: 7601  ; score ones += 1
0x29C: 360A  ; if ones == 10 skip next
0x29E: 12AA  ; jump to redraw score
0x2A0: 6600  ; ones = 0
0x2A2: 7701  ; tens += 1
0x2A4: 370A  ; if tens == 10 skip next
0x2A6: 12AA  ; jump to redraw score
0x2A8: 6700  ; tens = 0
0x2AA: 2260  ; draw new score
0x2AC: 3402  ; if speed == 2 skip next
0x2AE: 74FF  ; speed -= 1
0x2B0: 6E02  ; VE = beep length
0x2B2: FE18  ; sound timer = VE
0x2B4: 2300  ; respawn ball
0x2B6: 00EE

; redraw current ball or handle a miss at the bottom
0x2B8: 331F  ; if ball Y == 31 skip next
0x2BA: 12C0  ; redraw ball
0x2BC: 22C6  ; handle miss
0x2BE: 00EE
0x2C0: A380  ; redraw ball
0x2C2: D232
0x2C4: 00EE

; remove one life icon, then respawn or show game over
0x2C6: 3D03  ; if lives == 3 skip next
0x2C8: 12D8  ; jump to two-lives case
0x2CA: 6D02  ; lives = 2
0x2CC: A382  ; erase rightmost life icon
0x2CE: 680C
0x2D0: 6E00
0x2D2: D8E4
0x2D4: 12FA  ; respawn
0x2D6: 00EE

0x2D8: 3D02  ; if lives == 2 skip next
0x2DA: 12EA  ; jump to one-life case
0x2DC: 6D01  ; lives = 1
0x2DE: A382  ; erase middle life icon
0x2E0: 6806
0x2E2: 6E00
0x2E4: D8E4
0x2E6: 12FA  ; respawn
0x2E8: 00EE

0x2EA: 6D00  ; lives = 0
0x2EC: A382  ; erase leftmost life icon
0x2EE: 6800
0x2F0: 6E00
0x2F2: D8E4
0x2F4: 2316  ; game over
0x2F6: 00EE
0x2F8: 00EE

0x2FA: 2300  ; respawn helper
0x2FC: 00EE
0x2FE: 00EE

; choose a random column, reset to top, and draw the ball
0x300: C80F  ; V8 = random 0..15
0x302: 480F  ; if V8 != 15 skip next
0x304: 1300  ; retry so the ball never wraps at the right edge
0x306: 880E  ; * 2
0x308: 880E  ; * 4
0x30A: 8280  ; V2 = V8
0x30C: 6300  ; V3 = 0
0x30E: F415  ; delay timer = speed
0x310: A380  ; draw ball
0x312: D232
0x314: 00EE

; game over screen
0x316: 00E0  ; CLS
0x318: A386  ; G
0x31A: 6810  ; x = 16
0x31C: 6E08  ; y = 8
0x31E: D8E5
0x320: A38B  ; A
0x322: 6818  ; x = 24
0x324: D8E5
0x326: A390  ; M
0x328: 6820  ; x = 32
0x32A: D8E5
0x32C: A395  ; E
0x32E: 6828  ; x = 40
0x330: D8E5
0x332: A39A  ; O
0x334: 6818  ; x = 24
0x336: 6E10  ; y = 16
0x338: D8E5
0x33A: A39F  ; V
0x33C: 6820  ; x = 32
0x33E: D8E5
0x340: A395  ; E
0x342: 6828  ; x = 40
0x344: D8E5
0x346: A3A4  ; R
0x348: 6830  ; x = 48
0x34A: D8E5
0x34C: 134C  ; halt on game-over screen

; data at 0x380
0x380: 18 18             ; ball sprite
0x382: 18 3C 18 24       ; player sprite

; GAME OVER glyphs
0x386: 78 40 5C 44 78    ; G
0x38B: 38 44 7C 44 44    ; A
0x390: 44 6C 54 44 44    ; M
0x395: 7C 40 78 40 7C    ; E
0x39A: 38 44 44 44 38    ; O
0x39F: 44 44 28 28 10    ; V
0x3A4: 78 44 78 48 44    ; R
