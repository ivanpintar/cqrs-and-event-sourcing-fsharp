import React from 'react';
import { Table } from 'react-bootstrap';
import ProductRow from './ProductRow';
import AddToBasketModal from './AddToBasketModal';

class ProductList extends React.Component {
    constructor(props){
        super(props);
        this.state = { selectedProduct: null };
        this.showAddToBasketModal = this.showAddToBasketModal.bind(this);
    }    

    showAddToBasketModal(product) {
        this.setState({ selectedProduct: product });
    }

    render() {
        let { products, addToBasket } = this.props;
        let { selectedProduct } = this.state;
        let productRows = products.map(p => 
            ProductRow(p, () => this.showAddToBasketModal(p))
        );
        
        return (
            <div>
                <AddToBasketModal
                        product={selectedProduct}
                        addToBasket={addToBasket}
                        show={selectedProduct !== null}
                        onClose={() => this.showAddToBasketModal(null)}/>

                <Table  striped hover>
                    <thead>
                        <tr>
                            <th className='col-md-8'>Name</th>
                            <th className='col-md-2'>Price</th>
                            <th className='col-md-2'></th>
                        </tr>
                    </thead>
                    <tbody>{productRows}</tbody>
                </Table>
            </div>
        );
    }
}

export default ProductList;