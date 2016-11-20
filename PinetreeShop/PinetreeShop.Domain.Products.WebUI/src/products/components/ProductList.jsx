import React from 'react';
import { Button, Table } from 'react-bootstrap';
import ProductRow from './ProductRow';
import UpdateModal from './UpdateModal';
import AddModal from './AddModal';

class ProductList extends React.Component {
    constructor(props){
        super(props);
        this.state = { selectedProduct: null, showAddProductModal: false };
        this.showUpdateModal = this.showUpdateModal.bind(this);
    }    

    showUpdateModal(product) {
        this.setState({ selectedProduct: product });
    }

    showAddModal(show) {
        this.setState({ showAddProductModal: show })
    }

    render() {
        let { products, setQuantity, addProduct } = this.props;
        let { selectedProduct, showAddProductModal } = this.state;
        let productRows = products.map(p => ProductRow(p, () => this.showUpdateModal(p)));
        
        return (
            <div>
                <UpdateModal 
                        product={selectedProduct} 
                        show={selectedProduct !== null}
                        setQuantity={setQuantity}
                        onClose={() => this.showUpdateModal(null)}/>
                <AddModal
                        addProduct={addProduct}
                        show={showAddProductModal}
                        onClose={() => this.showAddModal(false)}/>

                <Table  striped hover>
                    <thead>
                        <tr>
                            <th className='col-md-3'></th>
                            <th className='col-md-3'>
                                Name
                            </th>
                            <th className='col-md-2'>Price</th>
                            <th className='col-md-2'>Qty</th>
                            <th className='col-md-2'>Res</th>
                        </tr>
                    </thead>
                    <tbody>{productRows}</tbody>
                    <tfoot>
                        <tr>
                            <td colSpan='5'>
                                <Button onClick={() => this.showAddModal(true)}>Add Product</Button>
                            </td>
                        </tr>
                    </tfoot>
                </Table>
            </div>
        );
    }
}

export default ProductList;