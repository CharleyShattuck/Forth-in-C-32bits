\ main.fs

\ split keyboard or one piece?
[  true constant TinyMod? ]
[ false constant split? ]
  split? [if] .( split TinyMod)     [then]
TinyMod? [if] .( one piece TinyMod) [then]
cr

target

\ Gemini PR mode
variable data 4 ramALLOT \ 6 bytes in all
: /data  data a! 5 #, for false c!+ next ; 

split? [if]
: initPortExpander
    $20 #, initMCP23017  $22 #, initMCP23017 ;
: @pins (  - n)
    $20 #, @MCP23017  $22 #, @MCP23017  16 #, lshift or ;
[then]
TinyMod? [if]
: initPortExpander  $20 #, initMCP23017 ;
: @pins (  - n)
    $20 #, @MCP23017 @GPIO 16 #, lshift or ;
[then]

: press (  - n)  false begin drop @pins until ;
: release ( n1 - n2)  begin @pins while or repeat drop ;
: scan (  - n)
    begin press 30 #, ms @pins if or release exit then drop drop again

: mark ( mask a)  data + dup >r c@ or r> c! ; 
: Gemini ( n)  /data $80 #, data c!
    dup $0100000 #, and if $40 #, 1 #, mark then drop \ S1
    dup $0200000 #, and if $10 #, 1 #, mark then drop \ T
    dup $0400000 #, and if $04 #, 1 #, mark then drop \ P
    dup $0800000 #, and if $01 #, 1 #, mark then drop \ H
    dup $1000000 #, and if $08 #, 2 #, mark then drop \ *
    dup $0008000 #, and if $02 #, 3 #, mark then drop \ F
    dup $0004000 #, and if $40 #, 4 #, mark then drop \ P
    dup $0002000 #, and if $10 #, 4 #, mark then drop \ L
    dup $0001000 #, and if $04 #, 4 #, mark then drop \ T
    dup $0000100 #, and if $01 #, 4 #, mark then drop \ D
    dup $0080000 #, and if $20 #, 1 #, mark then drop \ S2
    dup $0040000 #, and if $08 #, 1 #, mark then drop \ K
    dup $0020000 #, and if $02 #, 1 #, mark then drop \ W
    dup $0010000 #, and if $40 #, 2 #, mark then drop \ R
    dup $0000200 #, and if $04 #, 2 #, mark then drop \ *
    dup $0000001 #, and if $01 #, 3 #, mark then drop \ R
    dup $0000002 #, and if $20 #, 4 #, mark then drop \ B
    dup $0000004 #, and if $08 #, 4 #, mark then drop \ G
    dup $0000800 #, and if $02 #, 4 #, mark then drop \ S
    dup $0000400 #, and if $01 #, 5 #, mark then drop \ Z
    dup $0000008 #, and if $20 #, 2 #, mark then drop \ A
    dup $0000010 #, and if $10 #, 2 #, mark then drop \ O 
    dup $0000020 #, and if $40 #, 5 #, mark then drop \ #
    dup $0000040 #, and if $08 #, 3 #, mark then drop \ E
    dup $0000080 #, and if $04 #, 3 #, mark then drop \ U
    drop ; 

variable 'spit
: spit  'spit @ execute ; 
: >emit  ['] emit 'spit ! ; 
: >hc.  ['] hc. 'spit ! ; 
: send  data a! 5 #, for c@+ spit next ;

cvariable capping
: +caps  true capping c! ;
: -caps  false capping c! ;
cvariable spacing
: +space  true spacing c! ;
: -space  false spacing c! ;

: delay  2 #, ms ;

\ for backspacing with asterisk key
cvariable now
cvariable before
: +now  now c@ 1+ now c! ;

: emitHIDcaps ( c)  +now
    capping c@ if/ ( shift) $81 #, Keyboard.press -caps delay then
    Keyboard.press delay
    Keyboard.releaseAll delay ; 
: emitHID  +now Keyboard.write delay ;

\ : navigate  $86 #, Keyboard.press $b3 a #, emitHID
\    begin scan $20 #, = while/ $b3 #, emitHID repeat
\    Keyboard.releaseAll ;
\ : go-Gemini ( n - n)
\    begin
\        begin scan $1000220 #, - while $1000220 #, + Gemini send repeat
\        drop navigate
\    again

0 [if]
\ NKRO keyboard mode
cvariable former
: spew ( c - )
    dup Keyboard.press
    former c@ if dup Keyboard.release then
    drop former c! ; 

: send-NKRO ( n - )
    false former c!
    dup  $100000 #, and if/ [ char q ] #, spew then
             right c@ $06 #, = if/ char s #, emitHID exit then
    dup  $200000 #, and if/ [ char w ] #, spew then
    dup  $400000 #, and if/ [ char e ] #, spew then
    dup  $800000 #, and if/ [ char r ] #, spew then
    dup $1000000 #, and if/ [ char t ] #, spew then
\
    dup $8000 #, and if/ [ char u ] #, spew then
    dup $4000 #, and if/ [ char i ] #, spew then
    dup $2000 #, and if/ [ char o ] #, spew then
    dup $1000 #, and if/ [ char p ] #, spew then
    dup  $100 #, and if/ [ char [ ] #, spew then
\
    dup $80000 #, and if/ [ char a ] #, spew then
    dup $40000 #, and if/ [ char s ] #, spew then
    dup $20000 #, and if/ [ char d ] #, spew then
    dup $10000 #, and if/ [ char f ] #, spew then
    dup   $200 #, and if/ [ char g ] #, spew then
\
    dup  $01 #, and if/ [ char j ] #, spew then
    dup  $02 #, and if/ [ char k ] #, spew then
    dup  $04 #, and if/ [ char l ] #, spew then
    dup $800 #, and if/ [ char ; ] #, spew then
    dup $400 #, and if/ [ char ' ] #, spew then
\
    dup $08 #, and if/ [ char c ] #, spew then
    dup $10 #, and if/ [ char v ] #, spew then
    dup $20 #, and if/ [ char 3 ] #, spew then
    dup $40 #, and if/ [ char n ] #, spew then
    dup $80 #, and if/ [ char m ] #, spew then
    drop Keyboard.releaseAll ; 
: go-NKRO
    begin scan send-NKRO again
[then]

\ Jackdaw mode
variable stroke \ remember the last stroke
cvariable left
cvariable center
cvariable right
: ekey? (  - flag)  stroke @ $100 #, and 0= 0= ;
: ykey? (  - flag)  stroke @ $400 #, and 0= 0= ;
: *key1? (  - flag)  stroke @ $1000000 #, = ;
: *key2? (  - flag)  stroke @ $200 #, = ;
: *keys? (  - flag)  stroke @ $1000200 #, = ;
: !left ( c - )  left c@ or left c! ; 
: !right ( c - )  right c@ or right c! ; 
: !center ( c - )  center c@ or center c! ; 

: arrange ( n - )
    dup stroke !
    false dup right c! dup left c! center c!
    dup $100000 #, and if/ $01 #, !left then \ a
    dup $200000 #, and if/ $04 #, !left then \ c
    dup $400000 #, and if/ $10 #, !left then \ w
    dup $800000 #, and if/ $40 #, !left then \ n
    dup  $80000 #, and if/ $02 #, !left then \ s
    dup  $40000 #, and if/ $08 #, !left then \ t
    dup  $20000 #, and if/ $20 #, !left then \ h
    dup  $10000 #, and if/ $80 #, !left then \ r
    dup $8000 #, and if/ $01 #, !right then \ r
    dup $4000 #, and if/ $04 #, !right then \ l
    dup $2000 #, and if/ $10 #, !right then \ c
    dup $1000 #, and if/ $40 #, !right then \ t
    dup   $01 #, and if/ $02 #, !right then \ n
    dup   $02 #, and if/ $08 #, !right then \ g
    dup   $04 #, and if/ $20 #, !right then \ h
    dup  $800 #, and if/ $80 #, !right then \ s
    dup $08 #, and if/ $01 #, !center then \ i
    dup $10 #, and if/ $02 #, !center then \ e
    dup $20 #, and if/ $04 #, !center then \ a
    dup $40 #, and if/ $08 #, !center then \ o
    dup $80 #, and if/ $10 #, !center then \ u
    drop ; 

: display ( n - )
    dup $100000 #, and if/  char a #, emit then
    dup  $80000 #, and if/  char s #, emit then
    dup $200000 #, and if/  char c #, emit then
    dup  $40000 #, and if/  char t #, emit then
    dup $400000 #, and if/  char w #, emit then
    dup  $20000 #, and if/  char h #, emit then
    dup $800000 #, and if/  char n #, emit then
    dup  $10000 #, and if/  char r #, emit then
    char . #, emit
    stroke @ $1000000 #, and if/ char * #, emit space then
    dup $08 #, and if/  char i #, emit then
    dup $10 #, and if/  char e #, emit then
    dup $20 #, and if/  char a #, emit then
    dup $40 #, and if/  char o #, emit then
    dup $80 #, and if/  char u #, emit then
    stroke @ $200 #, and if/ space char * #, emit then
    char . #, emit
    dup $8000 #, and if/  char r #, emit then
    dup   $01 #, and if/  char n #, emit then
    dup $4000 #, and if/  char l #, emit then
    dup   $02 #, and if/  char g #, emit then
    dup $2000 #, and if/  char c #, emit then
    dup   $04 #, and if/  char h #, emit then
    dup $1000 #, and if/  char t #, emit then
    dup  $800 #, and if/  char s #, emit then
    drop cr ; 

\ left hand strings
-create l00 0 ,
-create l01 ," a"
-create l02 ," s"
-create l03 ," as"
-create l04 ," c"
-create l05 ," ac"
-create l06 ," sc"
-create l07 ," asc"
-create l08 ," t"
-create l09 ," at"
-create l0a ," st"
-create l0b ," ast"
-create l0c ," d"
-create l0d ," ad"
-create l0e ," g"
-create l0f ," ag"

-create l10 ," w" \ 1 , char w , 
-create l11 ," aw" \ 2 , char a , char w , 
-create l12 ," sw" \ 2 , char s , char w , 
-create l13 0 ,  \ asw
-create l14 ," p" \ 1 , char p , 
-create l15 ," ap" \ 2 , char a , char p , 
-create l16 ," sp" \ 2 , char s , char p , 
-create l17 ," ass" \ 3 , char a , char s , char s ,  \ ascn 
-create l18 ," tw" \ 2 , char t , char w , 
-create l19 ," att" \ 3 , char a , char t ,  char t ,  \ atw
-create l1a ," x" \ 1 , char x ,  \ stw
-create l1b ," ax" \ 2 , char a , char x , 
-create l1c ," dw" \ 2 , char d , char w ,  \ ctw
-create l1d ," add" \ 3 , char a , char d , char d ,  \ actw 
-create l1e ," gw" \ 2 , char g , char w ,  \ sctw
-create l1f ," agg" \ 3 , char a , char g , char g ,  \ asctw

-create l20 1 , char h ,
-create l21 2 , char a , char h ,
-create l22 2 , char s , char h ,
-create l23 3 , char a , char s , char h ,
-create l24 2 , char c , char h ,
-create l25 3 , char a , char c , char h ,
-create l26 3 , char s , char c , char h ,
-create l27 0 , \ asch
-create l28 2 , char t , char h ,
-create l29 3 , char a , char t , char h ,
-create l2a ," '"  \ sth
-create l2b 4 , char a , char s , char t , char h ,
-create l2c 1 , char f ,  \ cth
-create l2d 2 , char a , char f ,  \ acth
-create l2e 2 , char g , char h ,  \ scth
-create l2f 3 , char a , char g , char h ,  \ ascth

-create l30 2 , char w , char h ,
-create l31 3 , char a , char w , char h ,
-create l32 0 ,  \ swh
-create l33 0 ,  \ aswh
-create l34 2 , char p , char h ,  \ cwh
-create l35 3 , char a , char p , char h ,  \ acwh
-create l36 3 , char s , char p , char h ,  \ scwh
-create l37 4 , char a , char s , char p , char h ,  \ ascwh
-create l38 1 , char k ,  \ twh
-create l39 2 , char a , char k ,  \ atwh
-create l3a 2 , char s , char k ,  \ stwh
-create l3b 3 , char a , char s , char k ,  \ astwh
-create l3c 1 , char b ,  \ ctwh
-create l3d 2 , char a , char b ,  \ actwh
-create l3e 0 ,  \ sctwh
-create l3f 3 , char a , char b , char b ,  \ asctwh

-create l40 1 , char n ,
-create l41 2 , char a , char n ,
-create l42 2 , char s , char n ,
-create l43 3 , char a , char n , char n ,  \ asn
-create l44 1 , char z ,  \ cn
-create l45 2 , char a , char z ,  \ acn
-create l46 2 , char s , char s ,  \ scn
-create l47 3 , char a , char s , char s ,  \ ascn
-create l48 1 , char v ,  \ tn
-create l49 2 , char a , char v ,  \ atn
-create l4a 2 , char s , char v ,  \ stn
-create l4b 0 ,  \ astn
-create l4c 3 , char d , char e , char v ,  \ ctn
-create l4d 3 , char a , char d , char v ,  \ actn
-create l4e 2 , char g , char n ,  \ sctn
-create l4f 3 , char a , char g , char n ,  \ asctn

-create l50 1 , char m ,  \ wn
-create l51 2 , char a , char m ,  \ awn
-create l52 2 , char s , char m ,  \ swn
-create l53 3 , char a , char s , char m ,  \ aswn
-create l54 2 , char p , char n ,  \ cwn
-create l55 3 , char a , char m , char m ,  \ acwn
-create l56 0 ,  \ scwn
-create l57 3 , char a , char p , char p ,  \ ascwn
-create l58 1 , char j ,  \ twn
-create l59 2 , char a , char j ,  \ atwn
-create l5a 0 ,  \ stwn
-create l5b 0 ,  \ astwn
-create l5c 3 , char d , char e , char m ,  \ ctwn
-create l5d 3 , char a , char d , char m ,  \ actwn
-create l5e 0 ,  \ sctwn
-create l5f 2 , char a , char d , char j ,  \ asctwn

-create l60 1 , char y ,  \ hn
-create l61 2 , char a , char y ,  \ ahn
-create l62 2 , char s , char y ,  \ shn
-create l63 3 , char a , char s , char y ,  \ ashn
-create l64 2 , char c , char y ,  \ chn
-create l65 3 , char a , char c , char c ,  \ achn
-create l66 0 ,  \ schn
-create l67 0 ,  \ aschn
-create l68 2 , char t , char y ,  \ thn
-create l69 0 ,  \ athn
-create l6a 3 , char s , char t , char y ,  \ sthn
-create l6b 0 ,  \ asthn
-create l6c 2 , char d , char y ,  \ cthn
-create l6d 3 , char a , char f , char f ,  \ acthn
-create l6e 2 , char g , char y ,  \ scthn
-create l6f 3 , char a , char f , char t ,  \ ascthn

-create l70 2 , char m , char y ,  \ whn
-create l71 0 ,  \ awhn
-create l72 0 ,  \ swhn
-create l73 0 ,  \ aswhn
-create l74 2 , char p , char y ,  \ cwhn
-create l75 0 ,  \ acwhn
-create l76 3 , char s , char p , char y ,  \ scwhn
-create l77 5 , char a , char s , char p , char h , char y ,  \ ascwhn
-create l78 2 , char k , char n ,  \ twhn
-create l79 4 , char a , char c , char k , char n ,  \ atwhn
-create l7a 2 , char x , char y ,  \ stwhn
-create l7b 0 ,  \ astwhn
-create l7c 2 , char b , char y ,  \ ctwhn
-create l7d 3 , char a , char b , char y ,  \ actwhn
-create l7e 0 ,  \ scthnr
-create l7f 4 , char a , char f , char f , char l ,  \ ascthnr

-create l80 1 , char r , 
-create l81 2 , char a , char r ,
-create l82 3 , char s , char e , char r ,  \ sr
-create l83 3 , char a , char r , char r ,  \ asr 
-create l84 2 , char c , char r ,
-create l85 3 , char a , char c , char r ,
-create l86 3 , char s , char c , char r ,
-create l87 4 , char a , char s , char c , char r ,
-create l88 2 , char t , char r ,
-create l89 3 , char a , char t , char r ,
-create l8a 3 , char s , char t , char r ,
-create l8b 4 , char a , char s , char t , char r ,
-create l8c 2 , char d , char r ,  \ ctr
-create l8d 3 , char a , char d , char r ,  \ actr
-create l8e 2 , char g , char r ,  \ sctr
-create l8f 3 , char a , char g , char r ,  \ asctr

-create l90 2 , char w , char r ,  \ wr
-create l91 0 ,  \ awr
-create l92 0 ,  \ swr
-create l93 0 ,  \ aswr
-create l94 2 , char p , char r ,  \ cwr
-create l95 3 , char a , char p , char r ,  \ acwr
-create l96 3 , char s , char p , char r ,  \ scwr
-create l97 4 , char a , char p , char p , char r ,  \ ascwr 
-create l98 2 , char q , char u ,  \ twr
-create l99 4 , char a , char t , char t , char r ,  \ atwr
-create l9a 2 , char x , char r ,  \ stwr
-create l9b 0 ,  \ astwr
-create l9c 3 , char d , char e , char r ,  \ ctwr
-create l9d 4 , char a , char d , char d , char r ,  \ actwr
-create l9e 0 ,  \ sctwr
-create l9f 4 , char a , char g , char g , char r ,  \ asctwr

-create la0 2 , char r , char h ,  \ hr
-create la1 0 ,  \ ahr 
-create la2 3 , char s , char h , char r ,  \ shr
-create la3 0 ,  \ ashr
-create la4 3 , char c , char h , char r ,  \ chr
-create la5 4 , char a , char c , char c , char r ,  \ achr
-create la6 0 ,  \ schr
-create la7 0 ,  \ aschr
-create la8 3 , char t , char h , char r ,  \ thr
-create la9 0 ,  \ athr
-create laa 0 ,  \ sthr
-create lab 0 ,  \ asthr
-create lac 2 , char f , char r ,  \ cthr
-create lad 3 , char a , char f , char r ,  \ acthr
-create lae 0 ,  \ scthr
-create laf 4 , char a , char f , char f , char r ,  \ ascthr

-create lb0 0 ,  \ whr 
-create lb1 0 ,  \ awhr
-create lb2 0 ,  \ swhr
-create lb3 0 ,  \ aswhr
-create lb4 3 , char p , char h , char r ,  \ cwhr
-create lb5 4 , char a , char p , char h , char r ,  \ acwhr 
-create lb6 0 ,  \ scwhr 
-create lb7 0 ,  \ ascwhr
-create lb8 2 , char k , char r ,  \ twhr
-create lb9 0 ,  \ atwhr
-create lba 0 ,  \ stwhr
-create lbb 0 ,  \ astwhr
-create lbc 2 , char b , char r ,  \ ctwhr
-create lbd 3 , char a , char b , char r ,  \ actwhr
-create lbe 0 ,  \ sctwhr
-create lbf 4 , char a , char b , char b , char r ,  \ asctwhr

-create lc0 1 , char l ,  \ nr
-create lc1 2 , char a , char l ,  \ anr
-create lc2 2 , char s , char l ,  \ snr
-create lc3 3 , char a , char s , char l ,  \ asnr 
-create lc4 2 , char c , char l ,  \ cnr
-create lc5 0 ,  \ acnr
-create lc6 0 ,  \ scnr
-create lc7 0 ,  \ ascnr
-create lc8 1 , char q ,  \ tnr
-create lc9 2 , char a , char q ,  \ atnr
-create lca 2 , char s , char q ,  \ stnr
-create lcb 3 , char a , char s , char q ,  \ astnr
-create lcc 3 , char d , char e , char l ,  \ ctnr
-create lcd 3 , char a , char c , char q ,  \ actnr
-create lce 2 , char g , char l ,  \ sctnr
-create lcf 3 , char a , char g , char l ,  \ asctnr

-create ld0 2 , char m , char r ,  \ wnr
-create ld1 3 , char a , char l , char l ,  \ awnr
-create ld2 0 ,  \ swnr
-create ld3 0 ,  \ aswnr
-create ld4 2 , char p , char l ,  \ cwnr
-create ld5 3 , char a , char p , char l ,  \ acwnr
-create ld6 3 , char s , char p , char l ,  \ scwnr
-create ld7 4 , char a , char p , char p , char l ,  \ ascwnr
-create ld8 3 , char j , char e , char r ,  \ twnr
-create ld9 0 ,  \ atwnr
-create lda 4 , char s , char e , char r , char v ,  \ stwnr
-create ldb 0 ,  \ astwnr
-create ldc 0 ,  \ ctwnr
-create ldd 4 , char a , char d , char d , char l ,  \ actwnr
-create lde 0 ,  \ sctwnr
-create ldf 4 , char a , char g , char g , char l ,  \ asctwnr

-create le0 2 , char l , char y ,  \ hnr
-create le1 0 ,  \ ahnr
-create le2 3 , char s , char l , char y ,  \ shnr
-create le3 0 ,  \ ashnr
-create le4 3 , char c , char r , char y ,  \ chnr
-create le5 4 , char a , char c , char c , char l ,  \ achnr
-create le6 0 ,  \ schnr
-create le7 0 ,  \ aschnr
-create le8 3 , char t , char r , char y ,  \ thnr
-create le9 4 , char a , char t , char h , char l ,  \ athnr
-create lea 4 , char s , char t , char r , char y ,  \ sthnr
-create leb 0 ,  \ asthnr
-create lec 2 , char f , char l ,  \ cthnr
-create led 3 , char a , char f , char l ,  \ acthnr
-create lee 0 , \ scthnr
-create lef 4 , char a , char f , char f , char l ,  \ ascthnr

-create lf0 0 ,  \ whnr
-create lf1 0 ,  \ awhnr
-create lf2 0 ,  \ swhnr
-create lf3 0 ,  \ aswhnr
-create lf4 3 , char p , char h , char l ,  \ cwhnr
-create lf5 0 ,  \ acwhnr
-create lf6 0 ,  \ scwhnr
-create lf7 0 ,  \ ascwhnr
-create lf8 2 , char k , char l ,  \ twhnr
-create lf9 0 ,  \ atwhnr
-create lfa 0 ,  \ stwhnr
-create lfb 0 ,  \ astwhnr
-create lfc 2 , char b , char l ,  \ ctwhnr
-create lfd 3 , char a , char b , char l ,  \ actwhnr
-create lfe 0 ,  \ sctwhnr
-create lff 0 ,  \ asctwhnr

create left-table
    l00 , l01 , l02 , l03 , l04 , l05 , l06 , l07 ,
    l08 , l09 , l0a , l0b , l0c , l0d , l0e , l0f ,
    l10 , l11 , l12 , l13 , l14 , l15 , l16 , l17 ,
    l18 , l19 , l1a , l1b , l1c , l1d , l1e , l1f ,
    l20 , l21 , l22 , l23 , l24 , l25 , l26 , l27 ,
    l28 , l29 , l2a , l2b , l2c , l2d , l2e , l2f ,
    l30 , l31 , l32 , l33 , l34 , l35 , l36 , l37 ,
    l38 , l39 , l3a , l3b , l3c , l3d , l3e , l3f ,
    l40 , l41 , l42 , l43 , l44 , l45 , l46 , l47 ,
    l48 , l49 , l4a , l4b , l4c , l4d , l4e , l4f ,
    l50 , l51 , l52 , l53 , l54 , l55 , l56 , l57 ,
    l58 , l59 , l5a , l5b , l5c , l5d , l5e , l5f ,
    l60 , l61 , l62 , l63 , l64 , l65 , l66 , l67 ,
    l68 , l69 , l6a , l6b , l6c , l6d , l6e , l6f ,
    l70 , l71 , l72 , l73 , l74 , l75 , l76 , l77 ,
    l78 , l79 , l7a , l7b , l7c , l7d , l7e , l7f ,
    l80 , l81 , l82 , l83 , l84 , l85 , l86 , l87 ,
    l88 , l89 , l8a , l8b , l8c , l8d , l8e , l8f ,
    l90 , l91 , l92 , l93 , l94 , l95 , l96 , l97 ,
    l98 , l99 , l9a , l9b , l9c , l9d , l9e , l9f ,
    la0 , la1 , la2 , la3 , la4 , la5 , la6 , la7 ,
    la8 , la9 , laa , lab , lac , lad , lae , laf ,
    lb0 , lb1 , lb2 , lb3 , lb4 , lb5 , lb6 , lb7 ,
    lb8 , lb9 , lba , lbb , lbc , lbd , lbe , lbf ,
    lc0 , lc1 , lc2 , lc3 , lc4 , lc5 , lc6 , lc7 ,
    lc8 , lc9 , lca , lcb , lcc , lcd , lce , lcf ,
    ld0 , ld1 , ld2 , ld3 , ld4 , ld5 , ld6 , ld7 ,
    ld8 , ld9 , lda , ldb , ldc , ldd , lde , ldf ,
    le0 , le1 , le2 , le3 , le4 , le5 , le6 , le7 ,
    le8 , le9 , lea , leb , lec , led , lee , lef ,
    lf0 , lf1 , lf2 , lf3 , lf4 , lf5 , lf6 , lf7 ,
    lf8 , lf9 , lfa , lfb , lfc , lfd , lfe , lff ,

\ center, vowel strings
-create c00  0 ,
-create c01  ," i"
-create c02  ," e"
-create c03  ," ie"
-create c04  ," a"
-create c05  ," ia"
-create c06  ," ea"
-create c07  ," ai" \ swapped
-create c08  ," o"
-create c09  ," io"
-create c0a  ," eo"
-create c0b  ," oo" \ plus two opposite
-create c0c  ," ao"
-create c0d  ," oi" \ add a to swap
-create c0e  ," oa"
-create c0f  ," ei" \ two opposite to swap
-create c10  ," u"
-create c11  ," iu"
-create c12  ," eu"
-create c13  ," ue" \ add i to swap
-create c14  ," au"
-create c15  ," iau" \ hard to stroke
-create c16  ," eau" \ beautiful
-create c17  ," ua" \ very hard to stroke
-create c18  ," ou"
-create c19  ," iou"
-create c1a  ," ee" \ plus two opposite
-create c1b  ," ui" \ swapped
-create c1c  ," oe"
-create c1d  ," uo" \ very hard to stroke
-create c1e  ," I" \ all but i
-create c1f  ," ieaou" \ very hard to stroke

create center-table
    c00 , c01 , c02 , c03 , c04 , c05 , c06 , c07 ,
    c08 , c09 , c0a , c0b , c0c , c0d , c0e , c0f ,
    c10 , c11 , c12 , c13 , c14 , c15 , c16 , c17 ,
    c18 , c19 , c1a , c1b , c1c , c1d , c1e , c1f ,

\ right hand strings
-create r00  0 ,
-create r01  ," r"
-create r02  ," n"
-create r03  ," rn"
-create r04  ," l"
-create r05  ," rl"
-create r06  ," s"
-create r07  ," ll"
-create r08  ," g"
-create r09  ," rg"
-create r0a  ," ng"; 
-create r0b  ," gn"; 
-create r0c  ," lg"
-create r0d  0 ,
-create r0e  ," d"
-create r0f  ," dl"

-create r10  ," c"
-create r11  ," rc"
-create r12  ," nc"
-create r13  0 ,
-create r14  ," p"
-create r15  ," rp"
-create r16  ," sp"
-create r17  ," pl"
-create r18  ," b"
-create r19  ," rb"
-create r1a  ," gg"
-create r1b  0 ,
-create r1c  ," bl"
-create r1d  0 ,
-create r1e  ," ld"  \ CWS 25Jan20
-create r1f  ," lb"

-create r20  ," h"
-create r21  ," w"
-create r22  ," v"
-create r23  ," wn"
-create r24  ," z"
-create r25  ," wl"
-create r26  ," sh"
-create r27  ," lv"
-create r28  ," gh"
-create r29  ," rgh"
-create r2a  ," m"
-create r2b  ," rm"
-create r2c  ," x"
-create r2d  0 ,
-create r2e  ," sm"
-create r2f  ," lm"

-create r30 ," ch"
-create r31 ," rch"
-create r32 ," nch"
-create r33 ," rv"
-create r34 ," ph"
-create r35 0 ,
-create r36 0 ,
-create r37 ," lch"
-create r38 ," f"
-create r39 ," rf"
-create r3a ," mb"
-create r3b 0 ,
-create r3c ," lf"
-create r3d 0 ,
-create r3e ," mp"
-create r3f ," rd" \ CWS 3/29/20

-create r40 ," t"
-create r41 ," rt"
-create r42 ," nt"
-create r43 ," rnt"
-create r44 ," lt"
-create r45 0 ,
-create r46 ," st"
-create r47 ," rst"
-create r48 ," k"
-create r49 ," rk"
-create r4a ," nk"
-create r4b 0 ,
-create r4c ," kl"
-create r4d 0 ,
-create r4e ," sk"
-create r4f ," lk"

-create r50 ," ct"
-create r51 0 ,
-create r52 ," tion"
-create r53 ," ction"  \ CWS 25Jan20
-create r54 ," pt"
-create r55 0 ,
-create r56 ," nst"
-create r57 ," lp"
-create r58 ," ck"
-create r59 0 ,
-create r5a ," bt"
-create r5b 0 ,
-create r5c ," ckl"
-create r5d 0 ,
-create r5e 0 ,
-create r5f 0 ,

-create r60 ," th"
-create r61 ," rth"
-create r62 ," nth"
-create r63 ," wth"
-create r64 ," lth"
-create r65 0 ,
-create r66 0 ,
-create r67 0 ,
-create r68 ," ght"
-create r69 ," wk"
-create r6a ," ngth"
-create r6b 0 ,
-create r6c ," xt"
-create r6d 0 ,
-create r6e ," dth"
-create r6f 0 ,

-create r70 ," tch"
-create r71 0 ,
-create r72 0 ,
-create r73 0 ,
-create r74 ," pth"
-create r75 0 ,
-create r76 0 ,
-create r77 0 ,
-create r78 ," ft"
-create r79 0 ,
-create r7a 0 ,
-create r7b 0 ,
-create r7c 0 ,
-create r7d 0 ,
-create r7e ," mpt"
-create r7f 0 ,

-create r80 ," s"
-create r81 ," rs"
-create r82 ," ns"
-create r83 ," rns"
-create r84 ," ls"
-create r85 ," rls"
-create r86 ," ss"
-create r87 ," lls"
-create r88 ," gs"
-create r89 ," rgs"
-create r8a ," ngs"
-create r8b ," gns"
-create r8c 0 ,
-create r8d 0 ,
-create r8e ," ds"
-create r8f 0 ,

-create r90 ," cs"
-create r91 ," rcs"
-create r92 ," nces"
-create r93 0 ,
-create r94 ," ps"
-create r95 ," rps"
-create r96 ," sps"
-create r97 ," ples"
-create r98 ," bs"
-create r99 ," rbs"
-create r9a ," ggs"
-create r9b 0 ,
-create r9c ," bles"
-create r9d 0 ,
-create r9e ," lds"
-create r9f ," lbs"

-create ra0 ," hs"
-create ra1 ," ws"
-create ra2 ," ves"
-create ra3 ," wns"
-create ra4 ," zes"
-create ra5 ," wls"
-create ra6 ," shes"
-create ra7 ," lves"
-create ra8 ," ghs"
-create ra9 0 ,
-create raa ," ms"
-create rab ," rms"
-create rac ," xes"
-create rad 0 ,
-create rae ," sms"
-create raf ," lms"

-create rb0 ," d"
-create rb1 ," rd"
-create rb2 ," nd"
-create rb3 ," wd"
-create rb4 ," phs"
-create rb5 ," rld"
-create rb6 0 ,
-create rb7 ," ld"
-create rb8 ," dg"
-create rb9 0 ,
-create rba ," mbs"
-create rbb 0 ,
-create rbc 0 ,
-create rbd 0 ,
-create rbe ," mps"
-create rbf ," dd"

-create rc0 ," ts"
-create rc1 ," rts"
-create rc2 ," nts"
-create rc3 0 ,
-create rc4 ," lts"
-create rc5 0 ,
-create rc6 ," sts"
-create rc7 ," rsts"
-create rc8 ," ks"
-create rc9 ," rks"
-create rca ," nks"
-create rcb 0 ,
-create rcc 0 ,
-create rcd 0 ,
-create rce ," sks"
-create rcf ," lks"

-create rd0 ," cts"
-create rd1 0 ,
-create rd2 0 ,
-create rd3 0 ,
-create rd4 0 ,
-create rd5 0 ,
-create rd6 0 ,
-create rd7 ," lps"
-create rd8 ," cks"
-create rd9 0 ,
-create rda ," bts"
-create rdb 0 ,
-create rdc ," ckles"
-create rdd 0 ,
-create rde 0 ,
-create rdf 0 ,

-create re0 ," ths"
-create re1 ," rths"; 
-create re2 ," nths"
-create re3 ," wths"
-create re4 0 ,
-create re5 0 ,
-create re6 0 ,
-create re7 0 ,
-create re8 ," ghts"
-create re9 ," wks"
-create rea ," ngths"
-create reb 0 ,
-create rec 0 ,
-create red 0 ,
-create ree ," dths"
-create ref 0 ,

-create rf0 ," ds"
-create rf1 ," rds"
-create rf2 ," nds"
-create rf3 ," wds"
-create rf4 ," pths"
-create rf5 ," rlds"
-create rf6 0 ,
-create rf7 ," lds"
-create rf8 0 ,
-create rf9 0 ,
-create rfa 0 ,
-create rfb 0 ,
-create rfc 0 ,
-create rfd 0 ,
-create rfe ," mpts"
-create rff 0 ,

create right-table
  r00 , r01 , r02 , r03 , r04 , r05 , r06 , r07 ,
  r08 , r09 , r0a , r0b , r0c , r0d , r0e , r0f ,
  r10 , r11 , r12 , r13 , r14 , r15 , r16 , r17 ,
  r18 , r19 , r1a , r1b , r1c , r1d , r1e , r1f ,
  r20 , r21 , r22 , r23 , r24 , r25 , r26 , r27 ,
  r28 , r29 , r2a , r2b , r2c , r2d , r2e , r2f ,
  r30 , r31 , r32 , r33 , r34 , r35 , r36 , r37 ,
  r38 , r39 , r3a , r3b , r3c , r3d , r3e , r3f ,
  r40 , r41 , r42 , r43 , r44 , r45 , r46 , r47 ,
  r48 , r49 , r4a , r4b , r4c , r4d , r4e , r4f ,
  r50 , r51 , r52 , r53 , r54 , r55 , r56 , r57 ,
  r58 , r59 , r5a , r5b , r5c , r5d , r5e , r5f ,
  r60 , r61 , r62 , r63 , r64 , r65 , r66 , r67 ,
  r68 , r69 , r6a , r6b , r6c , r6d , r6e , r6f ,
  r70 , r71 , r72 , r73 , r74 , r75 , r76 , r77 ,
  r78 , r79 , r7a , r7b , r7c , r7d , r7e , r7f ,
  r80 , r81 , r82 , r83 , r84 , r85 , r86 , r87 ,
  r88 , r89 , r8a , r8b , r8c , r8d , r8e , r8f ,
  r90 , r91 , r92 , r93 , r94 , r95 , r96 , r97 ,
  r98 , r99 , r9a , r9b , r9c , r9d , r9e , r9f ,
  ra0 , ra1 , ra2 , ra3 , ra4 , ra5 , ra6 , ra7 ,
  ra8 , ra9 , raa , rab , rac , rad , rae , raf ,
  rb0 , rb1 , rb2 , rb3 , rb4 , rb5 , rb6 , rb7 ,
  rb8 , rb9 , rba , rbb , rbc , rbd , rbe , rbf ,
  rc0 , rc1 , rc2 , rc3 , rc4 , rc5 , rc6 , rc7 ,
  rc8 , rc9 , rca , rcb , rcc , rcd , rce , rcf ,
  rd0 , rd1 , rd2 , rd3 , rd4 , rd5 , rd6 , rd7 ,
  rd8 , rd9 , rda , rdb , rdc , rdd , rde , rdf ,
  re0 , re1 , re2 , re3 , re4 , re5 , re6 , re7 ,
  re8 , re9 , rea , reb , rec , red , ree , ref ,
  rf0 , rf1 , rf2 , rf3 , rf4 , rf5 , rf6 , rf7 ,
  rf8 , rf9 , rfa , rfb , rfc , rfd , rfe , rff ,

: typeHID ( a - )
    p! @p+ 1- -if drop exit then for @p+ emitHIDcaps next ;
: spaceHID  32 #, emitHID ;
: crHID  13 #, emitHID  10 #, emitHID ;
: escapeHID  $b1 #, emitHID ;
: backspaceHID  $08 #, emitHID ;
: right-thumb ( c -  flag)  center c@ $18 #, and = ;
: Emily ( c - )
    center c@ 1 #, and if/ spaceHID then
    emitHID
    center c@ 2 #, and if/ spaceHID then
    r> drop ; \ don't need to exit after Emily
: (digit) ( n - c)
    $0f #, and $0a #, - -if
        $3a #, + exit then $61 #, + ;
: backspaces
    before c@ 0= if/ backspaceHID exit then
    before c@ begin 
        $08 #, Keyboard.write delay 1- while repeat
    drop ;

: word-left  $80 #, Keyboard.press delay
    $d8 #, Keyboard.write delay Keyboard.releaseAll ;
: word-right  $80 #, Keyboard.press delay
    $d7 #, Keyboard.write delay Keyboard.releaseAll ;

: write  now c@ before c!  false now c!
    right c@ $8c #, = if/ +caps -space then   \ OXOO
    *key1? *key2? or if/ backspaces exit then \ OXOX
    left c@ 0= if/  \ endings
         center c@ 0= if/
             right c@ $06 #, =  right c@ $80 #, = or if/
                char s #, emitHID exit then
             right c@ $0e #, = if/
                char e #, emitHID char d #, emitHID exit then
             right c@ $0a #, = if/
                char i #, emitHID  char n #, emitHID
                char g #, emitHID  exit then
             right c@ $86 #, = if/
                char e #, emitHID  char s #, emitHID exit then
        then
    then
    left c@ $b0 #, = if/ \ modifiers                  \ OOXO 
        right c@ 0= if/ Keyboard.releaseAll exit then \ OOXX
        right c@ $02 #, and if/ $80 #, keyboard.press then \ control
        right c@ $08 #, and if/ $82 #, keyboard.press then \ alt
        right c@ $20 #, and if/ $83 #, keyboard.press then \ super
        right c@ $80 #, and if/ $81 #, keyboard.press then \ shift
        exit
    then                 \ OOOO
    left c@ $aa #, = if/ \ XXXX  movement
        right c@ $04 #, = if/ $da #, emitHID exit then \ up
        right c@ $08 #, = if/ $d9 #, emitHID exit then \ down
        right c@ $02 #, = if/ $d8 #, emitHID exit then \ left
        right c@ $20 #, = if/ $d7 #, emitHID exit then \ right
        right c@ $0a #, = if/ word-left  exit then \ word left
        right c@ $28 #, = if/ word-right exit then \ word right
        right c@ $15 #, = if/ $d2 #, emitHID exit then \ beginning of line
        right c@ $2a #, = if/ $d5 #, emitHID exit then \ end of line
        right c@ $26 #, = if/ $d3 #, emitHID exit then \ page up
        right c@ $19 #, = if/ $d6 #, emitHID exit then \ page down
    then
    \ OOXX          OOXX
    \ OOXX number   XOXX function key
    left c@ $f0 #, = if/ right c@ $0f #, and (digit) Emily then
    left c@ $f2 #, = if/ right c@ $0f #, and $c1 #, + emitHID exit then
    center c@ 0= if/  \ cr . , ! ?  common punctuation
        left c@ $a0 #, =  right c@ $0a #, = and if/ \ rh-ng
            crHID exit then
        left c@ $80 #, =  right c@ $08 #, = and if/ \ r-n
            crHID +caps exit then
        left c@ $14 #, =  right c@ $14 #, = and if/
            char . #, emitHID +caps exit then
        left c@ $28 #, =  right c@ $28 #, = and if/
            char , #, emitHID exit then
        left c@ $14 #, =  right c@ $28 #, = and if/
            char ! #, emitHID +caps exit then
        left c@ $28 #, =  right c@ $14 #, = and if/
            char ? #, emitHID +caps exit then
    then \ alphabet tables
    left c@ $5a #, - if/
        stroke @ $1000200 #, and 0= \ asterisk keys suppress space
            left c@ $2a #, - and 0= 0=  right c@ $8c #, - and
                if/ spaceHID then
        left c@ left-table + @p typeHID
        center c@ center-table + @p typeHID
        right c@ right-table + @p typeHID
        ekey? if/ char e #, emitHIDcaps then
        ykey? if/ char y #, emitHIDcaps then
        exit
    then \ Emily's symbols
    right c@ 0= stroke @ $1000200 #, and and if/  \ caps on
        true capping c! exit then
    right c@ 0= center c@ $18 #, = and if/ spaceHID exit then \ space
    right c@ $21 #, = if/ \ XOO
                          \ OOX
        center c@ 0=       if/ $b3 #, Emily then \ tab
        center c@ $08 #, = if/ $b2 #, Emily then \ backspace
        center c@ $10 #, = if/ $d4 #, Emily then \ delete
        center c@ $18 #, = if/ $b1 #, Emily then \ escape
    then
    right c@ $03 #, = if/    \ XOO
        char ! #, Emily then \ XOO
    right c@ $05 #, = if/     \ XXO
        char " #, Emily then  \ OOO 
    right c@ $33 #, = if/    \ XOX
        char # #, Emily then \ XOX
    right c@ $1e #, = if/     \ OXX
        char $ #, Emily then  \ XXO
    right c@ $0f #, = if/    \ XXO
        char % #, Emily then \ XXO
    right c@ $29 #, = if/     \ XOO
        char & #, Emily then  \ OXX
    right c@ $01 #, = if/    \ XOO
        char ' #, Emily then \ OOO
    right c@ $15 #, = if/       \ XXX
        $00 #, right-thumb if/  \ OOO \ (
            $28 #, Emily then
        $08 #, right-thumb if/        \ [
            $5b #, Emily then
        $10 #, right-thumb if/        \ <
            $3c #, Emily then
        $18 #, right-thumb if/        \ {
            $7b #, Emily then
    then
    right c@ $2a #, = if/       \ OOO 
        $00 #, right-thumb if/  \ XXX \ )
            char ) #, Emily then
        $08 #, right-thumb if/        \ ]
            char ] #, Emily then
        $10 #, right-thumb if/        \ >
            char > #, Emily then
        $18 #, right-thumb if/        \ }
            char } #, Emily then
    then
    right c@ $10 #, = if/    \ OOX
        char * #, Emily then \ OOO
    right c@ $20 #, = if/     \ OOO
        char + #, Emily then  \ OOX
    right c@ $08 #, = if/    \ OOO
        char , #, Emily then \ OXO
    right c@ $14 #, = if/     \ OXX
        char - #, Emily then  \ OOO
    right c@ $02 #, = if/    \ OOO
        char . #, Emily then \ XOO
    right c@ $06 #, = if/     \ OXO
        char / #, Emily then  \ XOO
    right c@ $30 #, = if/    \ OOX
        char : #, Emily then \ OOX
    right c@ $0a #, = if/     \ OOO
        char ; #, Emily then  \ XXO
    right c@ $3c #, = if/    \ OXX
        char = #, Emily then \ OXX
    right c@ $0d #, = if/     \ XXO
        char ? #, Emily then  \ OXO
    right c@ $3f #, = if/    \ XXX
        char @ #, Emily then \ XXX
    right c@ $09 #, = if/     \ XOO
        char \ #, Emily then  \ OXO
    right c@ $26 #, = if/    \ OXO
        char ^ #, Emily then \ XOX
    right c@ $28 #, = if/     \ OOO
        char _ #, Emily then  \ OXX
    right c@ $04 #, = if/    \ OXO
        char ` #, Emily then \ OOO
    right c@ $0c #, = if/     \ OXO
        char | #, Emily then  \ OXO
    right c@ $2d #, = if/    \ XXO
        char ~ #, Emily then \ OXX
    false capping c!  \ default
    ;
: Jackdaw  hex
    begin
        scan \ .s cr
        arrange \ .s cr
        write \ .s cr
        \ stroke @ dup u. .s cr
        \ display .s cr
        \ space .s cr cr
    again

: choose  7 #, @pin if/ Gemini send exit then
    arrange write ;
: go  begin scan choose again

: init  initPortExpander  initGPIO ;
turnkey decimal init Keyboard.begin
    false capping c! false now c!
\    >hc. interpret
\    >emit go-Gemini
\    go-NKRO
\    Jackdaw
    >emit go

