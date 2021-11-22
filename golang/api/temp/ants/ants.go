package ants

import (
	"sync"

	"api/temp/log"

	"github.com/panjf2000/ants/v2"
)

// Pool : goroutine pool
type Pool struct {
	pool          *ants.Pool
	wg            sync.WaitGroup
	isAutoRelease bool
	do            int
}

// New : 新增 pool
func New(size int, isAutoRelease bool, options ...ants.Option) (*Pool, error) {
	if size <= 0 {
		size = ants.DefaultAntsPoolSize
	}

	pool, err := ants.NewPool(size, options...)
	if err != nil {
		log.Error("new ants pool error", err, nil)
		return nil, err
	}

	return &Pool{pool: pool, isAutoRelease: isAutoRelease}, nil
}

// Do : 執行 pool 任務
func Do(pool *Pool, doFunc func()) *Pool {
	var err error
	if pool == nil {
		pool, err = New(0, true)
	}

	if err == nil {
		pool.Submit(doFunc)
	} else {
		doFunc()
	}

	return pool
}

// Submit : 執行任務
func (p *Pool) Submit(task func()) *Pool {
	// 紀錄執行次數
	p.do++
	p.wg.Add(1)
	// 執行函數
	err := p.pool.Submit(func() {
		task()
		p.finish()
	})

	if err != nil {
		log.Error("ants pool submit error", err, nil)
		p.finish()
	}

	return p
}

// finish : 完成任務
func (p *Pool) finish() {
	p.do--
	p.wg.Done()
	if p.do == 0 {
		if p.isAutoRelease {
			p.Release()
		}
	}
}

// Await : 等待所有任務完成
func (p *Pool) Await() *Pool {
	p.wg.Wait()
	return p
}

// Release : 關閉 Pool
func (p *Pool) Release() {
	p.pool.Release()
}
