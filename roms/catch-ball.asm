; Catch Ball
; Controls:
; - A moves left
; - D moves right
;
; Flow:
; - Title screen on boot
; - Gameplay with score and 3 lives
; - Game over screen shows high score and restarts on key press
;
; Register usage
; V0  player x
; V1  scratch x / player y when drawing
; V2  ball x
; V3  ball y
; V4  fall-speed delay
; V5  scratch / delay timer / wait-for-key target
; V6  score ones
; V7  score tens
; V8  high-score ones
; V9  high-score tens
; VA  left key nibble (A key)
; VB  right key nibble (D key)
; VC  left latch
; VD  right latch
; VE  lives

main:
    6800        ; V8 = high score ones
    6900        ; V9 = high score tens
    229A        ; call title
    2368        ; call start_game

loop:
    220E        ; call handle_input
    2250        ; call update_ball
    1208        ; jump loop

handle_input:
    EA9E        ; if A pressed
    2220        ;   move left
    EAA1        ; if A released
    6C00        ;   clear left latch
    EB9E        ; if D pressed
    223A        ;   move right
    EBA1        ; if D released
    6D00        ;   clear right latch
    00EE        ; return

move_left:
    3C00        ; if left latch == 0 skip next
    00EE        ; return
    6C01        ; set left latch
    4000        ; if player x != 0 skip next
    00EE        ; return at edge
    A502        ; player sprite
    611C        ; y = 28
    D014        ; erase player
    70FC        ; x -= 4
    A502
    611C
    D014        ; redraw player
    00EE

move_right:
    3D00        ; if right latch == 0 skip next
    00EE
    6D01        ; set right latch
    4038        ; if player x != 56 skip next
    00EE        ; return at edge
    A502
    611C
    D014        ; erase player
    7004        ; x += 4
    A502
    611C
    D014        ; redraw player
    00EE

update_ball:
    F507        ; V5 = delay timer
    3500        ; if timer == 0 skip next
    00EE        ; return
    A500        ; ball sprite
    D232        ; erase ball
    7301        ; ball y += 1
    F415        ; delay timer = speed
    611C        ; player y = 28
    5310        ; if ball y == player y skip next
    128C        ; jump check_bottom
    5200        ; if ball x == player x skip next
    128C        ; jump check_bottom
    238E        ; erase score
    7601        ; score ones += 1
    360A        ; if ones == 10 skip next
    127E        ; jump score_redraw
    6600        ; ones = 0
    7701        ; tens += 1
    370A        ; if tens == 10 skip next
    127E        ; jump score_redraw
    6700        ; tens = 0

score_redraw:
    238E        ; draw score
    3402        ; if speed == 2 skip next
    74FF        ; speed -= 1
    6502        ; beep length
    F518        ; sound timer = 2
    23E2        ; spawn next ball
    00EE

check_bottom:
    331F        ; if ball y == 31 skip next
    1294        ; jump redraw_ball
    23B0        ; remove life / maybe game over
    00EE

redraw_ball:
    A500
    D232        ; redraw ball
    00EE

title:
    00E0        ; CLS

    ; CATCH
    A506 610A 6502 D155
    A50B 6112 6502 D155
    A510 611A 6502 D155
    A506 6122 6502 D155
    A515 612A 6502 D155

    ; BALL
    A51A 6110 6508 D155
    A50B 6118 6508 D155
    A51F 6120 6508 D155
    A51F 6128 6508 D155

    ; A LEFT
    A50B 6106 6510 D155
    A51F 6118 6510 D155
    A529 6120 6510 D155
    A52E 6128 6510 D155
    A510 6130 6510 D155

    ; D RIGHT
    A524 6106 6516 D155
    A533 6118 6516 D155
    A538 6120 6516 D155
    A53D 6128 6516 D155
    A515 6130 6516 D155
    A510 6138 6516 D155

    ; no-op padding where the old PRESS label used to be
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000

    F50A        ; wait for key press
    00EE

start_game:
    00E0        ; CLS
    6018        ; player x = 24
    611C        ; player y = 28
    6408        ; fall speed = 8
    6600        ; score ones = 0
    6700        ; score tens = 0
    6A07        ; A key nibble
    6B09        ; D key nibble
    6C00        ; clear left latch
    6D00        ; clear right latch
    6E03        ; lives = 3
    6500        ; stop any leftover sound
    F518        ; sound timer = 0
    A502
    D014        ; draw player
    238E        ; draw score
    239E        ; draw lives
    23E2        ; spawn ball
    00EE

draw_score:
    6500        ; y = 0
    F729        ; tens font
    6134        ; x = 52
    D155
    F629        ; ones font
    613A        ; x = 58
    D155
    00EE

draw_lives:
    6500        ; y = 0
    A502        ; player sprite icon
    6100        ; x = 0
    D154
    6106        ; x = 6
    D154
    610C        ; x = 12
    D154
    00EE

remove_life:
    3E03        ; if lives == 3 skip next
    13C2        ; jump miss_two
    6E02        ; lives = 2
    A502
    610C        ; erase right icon
    6500
    D154
    23E2        ; respawn
    00EE

miss_two:
    3E02        ; if lives == 2 skip next
    13D4        ; jump miss_last
    6E01        ; lives = 1
    A502
    6106        ; erase middle icon
    6500
    D154
    23E2
    00EE

miss_last:
    6E00        ; lives = 0
    A502
    6100        ; erase left icon
    6500
    D154
    23F8        ; game over
    00EE

spawn_ball:
    C50F        ; random 0..15
    450F        ; if != 15 skip next
    13E2        ; retry so it stays on screen
    850E        ; * 2
    850E        ; * 4
    8250        ; ball x = V5
    6300        ; ball y = 0
    F415        ; delay timer = speed
    A500
    D232        ; draw ball
    00EE

game_over:
    9790        ; if score tens != high tens skip next
    1406        ; jump go_tens_equal
    8570        ; temp = score tens
    8595        ; temp -= high tens
    3F01        ; if score tens > high tens skip next
    1416        ; jump go_no_update
    1412        ; jump go_update

go_tens_equal:
    9680        ; if score ones != high ones skip next
    1416        ; jump go_no_update
    8560        ; temp = score ones
    8585        ; temp -= high ones
    3F01        ; if score ones > high ones skip next
    1416        ; jump go_no_update

go_update:
    8860        ; high ones = score ones
    8970        ; high tens = score tens

go_no_update:
    00E0        ; CLS

    ; GAME
    A53D 610C 6506 D155
    A50B 6114 6506 D155
    A542 611C 6506 D155
    A529 6124 6506 D155

    ; OVER
    A547 610C 650E D155
    A54C 6114 650E D155
    A529 611C 650E D155
    A533 6124 650E D155

    ; HI
    A515 610E 6516 D155
    A538 6116 6516 D155

    ; High score digits
    6516        ; y = 22
    F929        ; high tens font
    6126        ; x = 38
    D155
    F829        ; high ones font
    612E        ; x = 46
    D155

    ; no-op padding where the old PRESS label used to be
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000
    0000

    F50A        ; wait for key press
    2368        ; restart game
    00EE

; Data at 0x500
0x500: 18 18             ; ball sprite
0x502: 18 3C 18 24       ; player sprite

; Custom 5-byte glyphs
0x506: 3C 40 40 40 3C    ; C
0x50B: 18 24 3C 24 24    ; A
0x510: 7E 18 18 18 18    ; T
0x515: 24 24 3C 24 24    ; H
0x51A: 38 24 38 24 38    ; B
0x51F: 20 20 20 20 3C    ; L
0x524: 38 24 24 24 38    ; D
0x529: 3C 20 38 20 3C    ; E
0x52E: 3C 20 38 20 20    ; F
0x533: 38 24 38 28 24    ; R
0x538: 3C 08 08 08 3C    ; I
0x53D: 3C 20 2C 24 3C    ; G
0x542: 24 3C 3C 24 24    ; M
0x547: 18 24 24 24 18    ; O
0x54C: 24 24 24 18 18    ; V
0x551: 38 24 38 20 20    ; P
0x556: 1C 20 18 04 38    ; S
