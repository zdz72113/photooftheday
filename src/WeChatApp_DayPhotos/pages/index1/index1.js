//index.js
//获取应用实例
const app = getApp()
const dayjs = require('dayjs');
import Toast from '../../miniprogram_npm/vant-weapp/toast/toast';
const {
  qrcode
} = require('../../config.js');
const {
  requestData,
  previewImage,
  postFormId,
  downloadImage,
  writeText,
  writePhotosAlbum
} = require('../../utils/util.js');
Page({
  data: {
    loading: false,
    isEnd: false,
    todayInfo: {},
    imgList: [],
    totalPage: 0,
    total: 0,
    currentPage: 0,
    show: false,
    actions: [
    //   {
    //   name: "分享",
    //   type: "share"
    // }, 
    {
      name: "下载图片",
      type: "img_url"
    }, {
      name: "下载高清图片",
      type: "url"
    }]
  },
  onLoad: function() {
    this.getImgList();
  },
  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function() {
    if (this.data.currentPage >= this.data.totalPage) {
      this.setData({
        isEnd: true
      })
      return false;
    }
    this.setData({
      currentPage: this.data.currentPage + 1
    });
    this.getImgList();
  },
  // 转发设置
  onShareAppMessage: function() {
    return {
      title: this.data.todayInfo.img_title,
      imageUrl: 'https:' + this.data.todayInfo.imt_url_480
    }
  },
  postFormId,
  // 获取图片列表
  getImgList() {
    this.setData({
      loading: true
    });
    requestData('api/Photos/GetPhotos', {
      PageIndex: this.data.currentPage,
      PageSize: 5,
      Source: 2
    }).then(d => {
      setTimeout(_ => {
        console.log(d);
        // if (!this.data.todayInfo.day) {
        //   const todayInfo = (this.data.imgList.length ? this.data.imgList : d.aaData)[0];
        //   this.setData({
        //     todayInfo: {
        //       day: dayjs(todayInfo.img_time).format('DD'),
        //       date: dayjs(todayInfo.img_time).format('MMM.YYYY'),
        //       img_title: todayInfo.img_title,
        //       img_url: 'https:' + todayInfo.img_url,
        //       img_url_480: 'https:' + todayInfo.img_url_480,
        //       img_time: todayInfo.img_time
        //     }
        //   })
        // }
        this.setData({
          imgList: [...this.data.imgList, ...d.items.map(v => {
            v.img_date = dayjs(v.date).format('YYYY年MM月DD日');
            v.img_url = v.url.replace('.jpg', '.adapt.676.1.jpg');
            v.img_title = v.description + '(' + v.title + ')';
            return v;
          })],
          currentPage: d.currentPage,
          totalPage: d.totalPages,
          total: d.totalItems,
          loading: false,
          isEnd: (d.currentPage + 1) >= d.totalPage
        });
      }, 500);
    })
  },
  // 预览
  handleClickPreviewImage(e) {
    const {
      current
    } = e.currentTarget.dataset;
    previewImage(current, this.data.imgList.map(v => {
      return v.img_url;
    }));
  },
  // 长按触发上拉菜单
  handleLongTouch(e) {
    
    this.setData({
      show: !this.data.show
    });
    // 不存在inginfo,隐藏上拉菜单
    if (!e.currentTarget.dataset.imginfo) {
      return false;
    }
    // 获取当前选中图片信息
    const currentImgInfo = e.currentTarget.dataset.imginfo;
    this.setData({
      actions: [
      //   {
      //   name: "分享",
      //   func: _ => {
      //     this.handleClickGenerateShareImg(currentImgInfo);
      //     this.setData({
      //       show: false
      //     })
      //   }
      // }, 
      {
        name: "下载图片",
        func: _ => {
          downloadImage(currentImgInfo.img_url).then(filePath => {
            wx.saveImageToPhotosAlbum({
              filePath,
              success: _ => Toast('下载成功,请在相册查看相关图片.')
            })
          })
          this.setData({
            show: false
          })
        }
      }, 
      {
        name: "下载高清图片",
        func: _ => {
          downloadImage(currentImgInfo.url).then(filePath => {
            wx.saveImageToPhotosAlbum({
              filePath,
              success: _ => Toast('下载成功,请在相册查看相关图片.')
            })
          })
          this.setData({
            show: false
          })
        }
      }]
    })
  },
  onSelect(e) {
    e.detail.func();
  },
  // 点击分享到朋友圈
  handleClickGenerateShareImg(imgInfo) {
    writePhotosAlbum().then(_ => {
      wx.showLoading({
        title: '生成中,请稍后...',
      })
      const defaultTextY = 400;
      const map = {
        // day: {
        //   text: dayjs(imgInfo.img_date).format('DD'),
        //   x: 15,
        //   fontSize: 48,
        //   align: "left"
        // },
        date: {
          text: dayjs(imgInfo.img_date).format('DD MMM.YYYY'),
          x: 15,
          y: defaultTextY + 30,
          fontSize: 16,
          align: "left"
        },
        title: {
          text: imgInfo.img_title,
          x: 15,
          y: defaultTextY + 55,
          fontSize: 16,
          align: "left"
        },
      }
      let bg = imgInfo.img_url;
      let ctx = wx.createCanvasContext("myCanvas", this);
      Promise.all([downloadImage(bg), downloadImage(qrcode)]).then(([bg, qrcode]) => {
        ctx.font = "16px PingFangSC-Light sans-serif";
        ctx.fillStyle = "#FFF";
        ctx.drawImage(bg, 0, 0, 375, 500);
        ctx.drawImage(qrcode, 375 - 80 - 15, 340, 80, 80);

        let arr = [];
        for (var i in map) {
          arr.push(map[i]);
        }
        // 441
        arr.reduce((prevY, next) => {
          if (next.y) prevY = next.y;
          let y = next.marginTop ? prevY + (next.fontSize || 12) + next.marginTop : prevY;
          return writeText({
            ctx,
            text: next.text,
            x: next.x,
            y,
            align: next.align,
            fontSize: next.fontSize,
            maxWidth: next.maxWidth
          });
        }, defaultTextY);
        ctx.draw(true, _ => {
          console.log('draw done');
          // 保存canvas到本地资源
          wx.canvasToTempFilePath({
            canvasId: "myCanvas",
            success(res) {
              // 隐藏loading
              wx.hideLoading();
              wx.saveImageToPhotosAlbum({
                filePath: res.tempFilePath,
                success: _ => Toast('下载成功,请在相册查看相关图片.')
              })
              wx.showToast({
                title: '长按保存图片后分享到朋友圈.',
                icon: 'none',
                duration: 2000,
                success() {
                  setTimeout(_ => {
                    // 图片预览
                    wx.previewImage({
                      urls: [res.tempFilePath],
                    });
                  }, 2000);
                }
              })
            },
            fail(e) {
              console.log(e)
            }
          }, this);
        });
      })
    })
  }
})