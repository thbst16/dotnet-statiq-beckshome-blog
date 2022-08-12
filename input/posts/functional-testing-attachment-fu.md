Title: Functional Testing Attachment_Fu
Published: 7/21/2007
Tags:
    - Technology Guides
---
I was performing functional tests on my models that employed Attachment_Fu this morning and thought it would be worthwhile to share the code since it was a bit of a hassle pulling it together. Kudos to Mike Subelsky for his introduction to functional testing Attachment_Fu. It got me going in the right direction. What proved difficult once again was the multi-model controller. Once I got over that hump, I was on my way. As you can see from all the detail in the HTTP POST below, that was not an entirely easy task.

```ruby
1	class ProductsControllerTest < Test::Unit::TestCase
2	...
3	def test_create_with_user    
4	num_products = Product.count    
5	imgdata = fixture_file_upload('/files/image.png', 'image/png')    
6	audiodata = fixture_file_upload('/files/sound.mp3', 'audio/mpeg')    
7	post :create, {:product => {
8	:name => "Widget",
9	:description => "A small tool-like item",
10	:weight => "3",
11	:price => "19.99",
12	:language_id => "1";
13	},
14	:image => {:uploaded_data => imgdata},
15	:audio => {:uploaded_data => audiodata} ,
16	:html => { :multipart => true }
17	},
18	{:user_id => users(:valid_active_user).id}
19	assert_response :redirect
20	assert_redirected_to :action => 'show'
21	assert_equal num_products + 1, Product.count
22	end
23	...
24	end
```